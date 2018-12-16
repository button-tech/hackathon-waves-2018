namespace WavesBot.UI.Pages.Documents
{
    using System.Threading.Tasks;
    using IAL;
    using Telegram.Bot.Types.ReplyMarkups;
    using ViewModels.Documents;

    public class CreateDocumentPage : BaseTelegramPage
    {
        private readonly CreateDocumentViewModel viewModel;

        public CreateDocumentPage(CreateDocumentViewModel viewModel) : base(viewModel)
        {
            this.viewModel = viewModel;
        }

        protected override async Task Default()
        {
            var url = await viewModel.GetUrl();
            var inlineMenu = new InlineMenu
            {
                Text = "Create document:",
                Keyboard =
                    new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new InlineKeyboardButton
                            {
                                Text = "Create",
                                CallbackData = CallBack.RootPage.ToString(),
                                Url = url
                            }
                        }
                    })
            };
            viewModel.SendInlineKeyboard.Execute(inlineMenu);
        }
    }
}