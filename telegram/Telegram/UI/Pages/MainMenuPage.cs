namespace WavesBot.UI.Pages
{
    using ViewModels;

    public class MainMenuPage : BaseTelegramPage
    {
        private readonly MainMenuViewModel viewModel;

        public MainMenuPage(MainMenuViewModel viewModel) : base(viewModel)
        {
            this.viewModel = viewModel;
        }
    }
}