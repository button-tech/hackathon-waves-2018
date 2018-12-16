namespace WavesBot.Controllers
{
    using System.Threading.Tasks;
    using Data;
    using IAL;
    using Microsoft.AspNetCore.Mvc;
    using Services;
    using Telegram.Bot;
    using Telegram.Bot.Types;
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

        public BlockChainController(BotService botService,
            GuidService guidService,
            AccountService accountService,
            PageNavigator navigator,
            UserProvider userProvider)
        {
            this.botService = botService;
            this.guidService = guidService;
            this.accountService = accountService;
            this.navigator = navigator;
            this.userProvider = userProvider;
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