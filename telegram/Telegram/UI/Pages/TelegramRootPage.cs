namespace WavesBot.UI.Pages
{
    using System;
    using ViewLib.BaseNavigation;

    public class TelegramRootPage : IRootPage
    {
        public Type RootPageType()
        {
            return typeof(MainMenuPage);
        }
    }
}