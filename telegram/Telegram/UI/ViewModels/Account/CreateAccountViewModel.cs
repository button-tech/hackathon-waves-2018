namespace WavesBot.UI.ViewModels.Account
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using Services;
    using ViewLib.BaseNavigation;
    using ViewLib.Helpers;
    using ViewLib.Repositories;
    using ViewLib.Services;

    public class CreateAccountViewModel : BaseTelegramViewModel
    {
        private readonly AccountService accountService;
        private readonly GuidService guidService;
        private readonly BlockChainConfiguration config;

        public CreateAccountViewModel(PageNavigator pageNavigator, SafeCommandFactory commandFactory,
            Func<DateTimeOffset> timeStamp, ILoggerService loggerService, IPropertiesRepository propertiesRepository,
            BotService botService, AccountService accountService, GuidService guidService,
            BlockChainConfiguration config, StateSynchronizer stateSynchronizer)
            : base(pageNavigator, commandFactory, timeStamp, loggerService, propertiesRepository, botService)
        {
            this.accountService = accountService;
            this.guidService = guidService;
            this.config = config;

            RemoteState = CommandFactory.Create(async () => { await stateSynchronizer.SetStartState(Identifier); });
        }

        public Safe RemoteState { get; }

        public override Task<CallBack[]> GetPageCallBacks()
        {
            return Task.FromResult(new[] {CallBack.CreateAccount});
        }

        public async Task<bool> CheckAccountExist()
        {
            return await accountService.IsRegisteredAsync(Identifier);
        }

        public async Task<string> GetCreateUrl()
        {
            var guid = await guidService.GenerateString(Identifier, NickName);

            return $"{config.BlockChainAddress}/create/?create={guid}";
        }
    }
}