namespace WavesBot.UI.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using Pages.Documents;
    using Services;
    using ViewLib.BaseNavigation;
    using ViewLib.Data;
    using ViewLib.Helpers;
    using ViewLib.Repositories;
    using ViewLib.Services;

    public class MainMenuViewModel : BaseTelegramViewModel
    {
        public MainMenuViewModel(PageNavigator pageNavigator, SafeCommandFactory commandFactory,
            Func<DateTimeOffset> timeStamp, ILoggerService loggerService, IPropertiesRepository propertiesRepository,
            BotService botService) : base(pageNavigator, commandFactory, timeStamp, loggerService, propertiesRepository,
            botService)
        {
            GoToCreateDocumentPage = commandFactory.Create(async () =>
            {
                await PageNavigator.PushPageAsync<ChoicePartnerNicknamePage>(Identifier, DialogContext,
                    NavigationType.PageToPage);
            });

            GoToMyDocumentsPage = commandFactory.Create(async () =>
            {
                await PageNavigator.PushPageAsync<MyDocumentsPage>(Identifier, DialogContext,
                    NavigationType.PageToPage);
            });
        }

        public Safe GoToCreateDocumentPage { get; }

        public Safe GoToMyDocumentsPage { get; }
    }
}