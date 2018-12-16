namespace WavesBot.UI.ViewModels.Documents
{
    using System;
    using System.Threading.Tasks;
    using Data;
    using Pages.Documents;
    using Repositories;
    using Services;
    using ViewLib.BaseNavigation;
    using ViewLib.Data;
    using ViewLib.Helpers;
    using ViewLib.Repositories;
    using ViewLib.Services;
    using ViewModels;

    public class ChoicePartnerNicknameViewModel : BaseTelegramViewModel
    {
        private readonly IUserDataRepository userDataRepository;

        public ChoicePartnerNicknameViewModel(PageNavigator pageNavigator, SafeCommandFactory commandFactory,
            Func<DateTimeOffset> timeStamp, ILoggerService loggerService, IPropertiesRepository propertiesRepository,
            BotService botService, IUserDataRepository userDataRepository) : base(pageNavigator, commandFactory,
            timeStamp, loggerService, propertiesRepository,
            botService)
        {
            this.userDataRepository = userDataRepository;

            GoToCreateDocument = commandFactory.Create(async () =>
                await PageNavigator.PushPageAsync<CreateDocumentPage>(Identifier, DialogContext,
                    NavigationType.PageToPage));
        }

        public async void SetUserPartnerNick(string nickname)
        {
            var user = await userDataRepository.ReadAsync(nickname);
            SetPersistentValueFor<CreateDocumentViewModel>(user, nameof(CreateDocumentViewModel.PartnerUser));
            await SaveChangesAsync();
        }

        public Safe GoToCreateDocument { get; }
    }
}