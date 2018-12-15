namespace ViewLib.Data
{
    using System.Threading.Tasks;

    public class BasePage
    {
        public BasePage(BaseViewModel viewModel)
        {
            BindingContent = viewModel;
        }

        public object BindingContent { get; }

        public virtual Task RenderPage()
        {
            return Task.CompletedTask;
        }
    }
}