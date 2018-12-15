namespace WavesBot.UI.ViewModels
{
    using System;
    using Services;
    using ViewLib.BaseNavigation;
    using ViewLib.Helpers;
    using ViewLib.Repositories;
    using ViewLib.Services;

    public class MainMenuViewModel : BaseTelegramViewModel
    {
        public MainMenuViewModel(PageNavigator pageNavigator, SafeCommandFactory commandFactory, Func<DateTimeOffset> timeStamp, ILoggerService loggerService, IPropertiesRepository propertiesRepository, BotService botService) : base(pageNavigator, commandFactory, timeStamp, loggerService, propertiesRepository, botService)
        {
        }
    }
}