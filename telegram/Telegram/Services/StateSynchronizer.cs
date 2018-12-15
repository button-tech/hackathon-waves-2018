namespace WavesBot.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using UI.Pages;
    using UI.Pages.Account;
    using UI.Pages.Documents;
    using UI.ViewModels;
    using ViewLib.BaseNavigation;
    using ViewLib.Data;

    public class StateSynchronizer
    {
        private readonly PageNavigator navigator;
        private readonly PageLocator pageLocator;
        private readonly AccountService accountService;

        public StateSynchronizer(PageNavigator navigator, PageLocator pageLocator,
            AccountService accountService)
        {
            this.navigator = navigator;
            this.pageLocator = pageLocator;
            this.accountService = accountService;
        }

        public async Task ResolveState(Update update)
        {
            await HandleCallback(update);
            
            if (update.Type != UpdateType.Message)
                return;

            if (update.Message.Type != MessageType.Text)
                return;
            
            await accountService.ReadUser(GetUserId(update));
            
            var isRegister = await RestoreRegistration(update);
            if (!isRegister)
                return;
            
            await RestoreState(update);
        }
        
        /// <summary>
        /// Восстанавливает станицу, на которой юзер был
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        private async Task RestoreState(Update update)
        {
            var identifier = GetUserId(update);

            var isMainMenu = await IsMainMenuCheck(update);
            if (!isMainMenu)
                await navigator.RestoreLastPage(identifier, update);
        }
        
        /// <summary>
        /// Доступ к MainMenu (то, что снизу) из любого места
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        private async Task<bool> IsMainMenuCheck(Update update)
        {
            var result = false;
            var identifier = GetUserId(update);
            
            if (update.Message.Text == "Мои документы")
            {
                result = true;
                await navigator.RemoveAllNavigationStack(identifier);
                await navigator.PushPageAsync<MyDocumentsPage>(identifier, update, NavigationType.PageToPage);
            }

            return result;
        }
        
        private async Task<bool> RestoreRegistration(Update update)
        {
            var identifier = GetUserId(update);
            var username = update.Message.From.Username;
            var isRegister = await accountService.IsRegisteredAsync(identifier);

            if (isRegister)
            {
                await accountService.UpdateNickNameAsync(identifier, username);
                return true;
            }

            await navigator.RemoveAllNavigationStack(identifier);
            await navigator.PushPageAsync<CreateAccountPage>(identifier, update);

            return false;
        }

        private async Task HandleCallback(Update update)
        {
            if (update.Type == UpdateType.CallbackQuery)
            {
            }
        }
        
        private static readonly Dictionary<string, Type> GlobalCallBacks = new Dictionary<string, Type>();

        public async Task Init()
        {
            var pages = GetAssemblyPageTypes();

            foreach (var pageType in pages)
            {
                var page = pageLocator.Resolve(pageType);

                if (!(page.BindingContent is BaseTelegramViewModel viewModel))
                    continue;

                var callBacks = await viewModel.GetPageCallBacks();
                foreach (var callBack in callBacks)
                {
                    GlobalCallBacks.Add(callBack.ToString(), pageType);
                }
            }
        }
        
        private async Task ResolveCallBackState(long identifier, object context, string callBackData)
        {
            var type = GlobalCallBacks[callBackData];

            await navigator.PushPageAsync(type, identifier, context, NavigationType.PageToPage);
        }
        
        private long GetUserId(Update update)
        {
            return update?.CallbackQuery?.Message?.Chat.Id ??
                   update?.Message?.Chat?.Id ??
                   throw new Exception("UserId not exist");
        }

        private string GetNickName(Update update)
        {
            return update?.CallbackQuery?.From?.Username ??
                   update?.Message?.From?.Username ??
                   string.Empty;
        }

        private string GetMessageText(Update update)
        {
            return update?.Message?.Text ?? update?.CallbackQuery?.Message.Text;
        }

        private static Type[] GetAssemblyPageTypes()
        {
            return typeof(BaseTelegramPage).GetTypeInfo()
                .Assembly.DefinedTypes
                .Where(typeInfo => typeInfo.IsClass && !typeInfo.IsAbstract && typeInfo.Name.Contains(@"Page")
                                   && typeInfo.BaseType == typeof(BaseTelegramPage))
                .Select(x => x.AsType()).ToArray();
        }

        public async Task SetStartState(long identifier)
        {
            var update = GenerateMessageUpdate(identifier);

            await navigator.RemoveAllNavigationStack(identifier);

            var isRegister = await RestoreRegistration(update);
            if (!isRegister)
                return;

            await RestoreState(update);
        }
        
        private static Update GenerateMessageUpdate(long identifier)
        {
            var update = new Update
            {
                Message = new Message
                {
                    From = new User
                    {
                        Id = (int)identifier
                    },
                    Chat = new Chat
                    {
                        Id = identifier
                    },
                    Text = "/start"
                }
            };
            return update;
        }
    }
}