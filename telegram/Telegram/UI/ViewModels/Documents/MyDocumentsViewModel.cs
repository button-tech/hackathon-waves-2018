namespace WavesBot.UI.ViewModels.Documents
{
    using System;
    using Services;
    using ViewLib.BaseNavigation;
    using ViewLib.Helpers;
    using ViewLib.Repositories;
    using ViewLib.Services;

    public class MyDocumentsViewModel : BaseTelegramViewModel
    {
        public MyDocumentsViewModel(PageNavigator pageNavigator, SafeCommandFactory commandFactory,
            Func<DateTimeOffset> timeStamp, ILoggerService loggerService, IPropertiesRepository propertiesRepository,
            BotService botService) : base(pageNavigator, commandFactory, timeStamp, loggerService, propertiesRepository,
            botService)
        {
        }
    }
}