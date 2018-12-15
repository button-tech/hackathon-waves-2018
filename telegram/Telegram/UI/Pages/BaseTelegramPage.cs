namespace WavesBot.UI.Pages
{
    using System.Threading.Tasks;
    using ViewLib.Data;
    using ViewModels;

    public class BaseTelegramPage : BasePage
    {
        private readonly BaseTelegramViewModel viewModel;

        public BaseTelegramPage(BaseTelegramViewModel viewModel)
            : base(viewModel)
        {
            this.viewModel = viewModel;
        }

        public bool IsAlwaysDefaultRender = false;
        
        public override Task RenderPage()
        {
            Navigation();

            if (viewModel.IsCallBack)
                Callback();
            else
                Message();

            if (viewModel.IsDefaultRenderNeed != null && (bool) viewModel.IsDefaultRenderNeed &&
                (viewModel.NavigationType != NavigationType.RestoreState || IsAlwaysDefaultRender))
                Default();

            return Task.CompletedTask;
        }

        protected virtual Task Message()
        {
            return Task.CompletedTask;
        }

        protected virtual Task Navigation()
        {
            return Task.CompletedTask;
        }

        protected virtual Task Default()
        {
            return Task.CompletedTask;
        }

        protected virtual Task Callback()
        {
            return Task.CompletedTask;
        }
    }
}