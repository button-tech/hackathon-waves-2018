namespace WavesBot.Controllers
{
    using System.Threading.Tasks;
    using Configuration;
    using Data;
    using IAL;
    using Microsoft.AspNetCore.Mvc;
    using Repositories;
    using Services;
    using Telegram.Bot;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using Telegram.Bot.Types.ReplyMarkups;
    using UI;
    using ViewLib.BaseNavigation;
    using ViewLib.Data;

    [Route("api/[controller]")]
    [ApiController]
    public class BlockChainController : Controller
    {
        private readonly BotService botService;
        private readonly GuidService guidService;
        private readonly AccountService accountService;
        private readonly PageNavigator navigator;
        private readonly UserProvider userProvider;
        private readonly IDocumentDataRepository documentDataRepository;
        private readonly IUserDataRepository userDataRepository;
        private readonly BlockChainConfiguration configuration;

        public BlockChainController(BotService botService,
            GuidService guidService,
            AccountService accountService,
            PageNavigator navigator,
            UserProvider userProvider,
            IDocumentDataRepository documentDataRepository,
            IUserDataRepository userDataRepository,
            BlockChainConfiguration configuration)
        {
            this.botService = botService;
            this.guidService = guidService;
            this.accountService = accountService;
            this.navigator = navigator;
            this.userProvider = userProvider;
            this.documentDataRepository = documentDataRepository;
            this.userDataRepository = userDataRepository;
            this.configuration = configuration;
        }

        private TelegramBotClient BotClient => botService.Client;

        [HttpPut("create/{guid}")]
        public async Task<object> CreateWallet([FromBody] AccountStamp accountStamp, string guid)
        {
            var isGuidValid = await guidService.ValidateString(guid);
            if (!isGuidValid)
            {
                var res = Json(new
                    {
                        error = "Guid is not exist",
                        result = default(string)
                    })
                    .Value;
                return NotFound(res);
            }

            var guidStamp = await guidService.GetGuidStamp(guid);
            await guidService.DeleteGuid(guid);

            var isUserExist = await accountService.IsExistAsync(guidStamp.Identifier);
            if (!isUserExist)
                await accountService.CreateTelegramUser(guidStamp.Identifier, guidStamp.NickName ?? string.Empty);

            var user = await userProvider.GetAsync(guidStamp.Identifier);

            if (user.RsaPublicKey != null)
            {
                await BotClient.SendTextMessageAsync(guidStamp.Identifier, "Sorry, account already exists");

                var res = Json(new
                    {
                        error = "Guid is not exist",
                        result = default(string)
                    })
                    .Value;

                return NotFound(res);
            }

            user.RsaPublicKey = accountStamp.RsaPublicKey;

            await userProvider.UpdateAsync(user);

            var update = GenerateBotUpdate(guidStamp);

            await BotClient.SendTextMessageAsync(guidStamp.Identifier, "Account successfully created");
            await navigator.PopToRootAsync(guidStamp.Identifier, update);

            return Json(new
            {
                error = default(string),
                result = "success"
            });
        }

        [HttpGet("documentData/{guid}")]
        public async Task<object> GetDocumentData(string guid)
        {
            var isGuidValid = await guidService.ValidateString(guid);
            if (!isGuidValid)
            {
                var res = Json(new
                    {
                        error = "Guid is not exist",
                        result = default(string)
                    })
                    .Value;
                return NotFound(res);
            }

            var guidStamp = await guidService.GetGuidStamp(guid);
            await guidService.DeleteGuid(guid);

            var waves = guidStamp.WavesData;
            return Json(new
            {
                publicKeyOwner = waves.MyRsaPublicKey,
                publicKeyPartner = waves.PartnerRsaPublicKey,
                nicknameOwner = waves.MyNickname,
                nicknamePartner = waves.PartnerNickname
            });
        }

        [HttpPost("documentData/{guid}")]
        public async Task<object> PostDocumentData([FromBody] DocumentDto documentDto, string guid)
        {
            var isGuidValid = await guidService.ValidateString(guid);
            if (!isGuidValid)
            {
                var res = Json(new
                    {
                        error = "Guid is not exist",
                        result = default(string)
                    })
                    .Value;
                return NotFound(res);
            }

            var guidStamp = await guidService.GetGuidStamp(guid);
            await guidService.DeleteGuid(guid);

            var waves = guidStamp.WavesData;

            var owner = await userDataRepository.ReadAsync(waves.MyNickname);
            var partner = await userDataRepository.ReadAsync(waves.PartnerNickname);

            var ownerData = new DocumentData
            {
                Identifier = owner.Identifier,
                DocumentId = documentDto.DocumentId,
            };

            var partnerData = new DocumentData
            {
                Identifier = partner.Identifier,
                DocumentId = documentDto.DocumentId,
            };

            await documentDataRepository.CreateAsync(ownerData);
            await documentDataRepository.CreateAsync(partnerData);

            var urlGuid = await guidService.GenerateString(partner.Identifier, partner.NickName);

            var inlineMenu = new InlineMenu
            {
                Text = "Подпишите транзакцию",
                Keyboard =
                    new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new InlineKeyboardButton
                            {
                                Text = "Подписать",
                                CallbackData = CallBack.RootPage.ToString(),
                                Url = $"{configuration.BlockChainAddress}/sign/?create={urlGuid}"
                            }
                        }
                    })
            };


            await botService.Client.SendTextMessageAsync(partner.Identifier, inlineMenu.Text, ParseMode.Markdown,
                replyMarkup: inlineMenu.Keyboard);

            await BotClient.SendTextMessageAsync(partner.Identifier,
                $"Документ отправлен {partner.NickName} на подпись");

            return Json(new
            {
                error = default(string),
                result = "success"
            });
        }

        [HttpGet("documentId/{guid}")]
        public async Task<object> GetDocumentId(string guid)
        {
            var isGuidValid = await guidService.ValidateString(guid);
            if (!isGuidValid)
            {
                var res = Json(new
                    {
                        error = "Guid is not exist",
                        result = default(string)
                    })
                    .Value;
                return NotFound(res);
            }

            var guidStamp = await guidService.GetGuidStamp(guid);

            var owner = await accountService.ReadUser(guidStamp.Identifier);

            var document = await documentDataRepository.ReadAsync(owner.Identifier);

            return Json(new
            {
                document.DocumentId
            });
        }

        [HttpGet("validator/{guid}")]
        public async Task<object> GuidValidator(string guid)
        {
            var isGuidValid = await guidService.ValidateString(guid);
            if (!isGuidValid)
            {
                var res = Json(new
                    {
                        error = "Guid is not exist",
                        result = default(string)
                    })
                    .Value;
                return NotFound(res);
            }

            var lifetime = await guidService.GetLifetime(guid);
            return Json(new
            {
                error = default(string),
                result = lifetime
            });
        }

        private static Update GenerateBotUpdate(TelegramGuidStamp guidStamp)
        {
            var update = new Update
            {
                Message = new Message
                {
                    From = new User
                    {
                        Id = (int) guidStamp.Identifier
                    },
                    Chat = new Chat
                    {
                        Id = guidStamp.Identifier
                    },
                    Text = "/start"
                }
            };
            return update;
        }
    }
}