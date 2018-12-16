namespace WavesBot.UI.ViewModels.Documents
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using Data;
    using Services;
    using ViewLib.BaseNavigation;
    using ViewLib.Data;
    using ViewLib.Helpers;
    using ViewLib.Repositories;
    using ViewLib.Services;
    using ViewModels;

    public class CreateDocumentViewModel : BaseTelegramViewModel
    {
        private readonly GuidService guidService;
        private readonly BlockChainConfiguration config;
        private readonly AccountService accountService;

        public CreateDocumentViewModel(PageNavigator pageNavigator, SafeCommandFactory commandFactory,
            Func<DateTimeOffset> timeStamp, ILoggerService loggerService, IPropertiesRepository propertiesRepository,
            BotService botService, GuidService guidService, BlockChainConfiguration config,
            AccountService accountService)
            : base(pageNavigator, commandFactory, timeStamp, loggerService, propertiesRepository, botService)
        {
            this.guidService = guidService;
            this.config = config;
            this.accountService = accountService;
        }

        public UserData PartnerUser => GetPersistentValue<UserData>();

        public async Task<string> GetUrl()
        {
            var myAccount = await accountService.ReadUser(Identifier);
            var guid = await guidService.GenerateString(Identifier, NickName, new WavesData()
            {
                MyNickname = myAccount.NickName,
                MyRsaPublicKey = myAccount.RsaPublicKey,
                PartnerNickname = PartnerUser.NickName,
                PartnerRsaPublicKey = PartnerUser.RsaPublicKey
            });

            return $"{config.BlockChainAddress}/upload/?create={guid}";
        }
    }
}