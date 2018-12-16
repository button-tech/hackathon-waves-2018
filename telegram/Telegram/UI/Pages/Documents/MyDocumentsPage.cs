namespace WavesBot.UI.Pages.Documents
{
    using System.Threading.Tasks;
    using IAL;
    using Telegram.Bot.Types.ReplyMarkups;
    using ViewModels.Documents;

    public class MyDocumentsPage : BaseTelegramPage
    {
        private readonly MyDocumentsViewModel viewModel;

        public MyDocumentsPage(MyDocumentsViewModel viewModel) : base(viewModel)
        {
            this.viewModel = viewModel;
        }

        protected override Task Default()
        {
            var inlineMenu = new InlineMenu
            {
                Text = "Document List:",
                Keyboard =
                    new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new InlineKeyboardButton
                            {
                                Text = "1 Doc",
                                CallbackData = CallBack.RootPage.ToString()
                            },
                            new InlineKeyboardButton
                            {
                                Text = "2 Doc",
                                CallbackData = CallBack.RootPage.ToString()
                            }
                        }
                    })
            };
            viewModel.SendInlineKeyboard.Execute(inlineMenu);
            return base.Default();
        }
    }
}