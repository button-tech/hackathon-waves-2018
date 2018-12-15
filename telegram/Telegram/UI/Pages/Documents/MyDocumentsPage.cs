namespace WavesBot.UI.Pages.Documents
{
    using ViewModels.Documents;

    public class MyDocumentsPage : BaseTelegramPage
    {
        private readonly MyDocumentsViewModel viewModel;

        public MyDocumentsPage(MyDocumentsViewModel viewModel) : base(viewModel)
        {
            this.viewModel = viewModel;
        }
        
        
    }
}