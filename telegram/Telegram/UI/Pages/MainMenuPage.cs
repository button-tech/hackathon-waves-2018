namespace WavesBot.UI.Pages
{
    using System.Threading.Tasks;
    using IAL;
    using Telegram.Bot.Types.ReplyMarkups;
    using ViewModels;

    public class MainMenuPage : BaseTelegramPage
    {
        private readonly MainMenuViewModel viewModel;

        public MainMenuPage(MainMenuViewModel viewModel) : base(viewModel)
        {
            this.viewModel = viewModel;
        }

        protected override Task Default()
        {
            var replyMenu = new ReplyMenu
            {
                Text = "Main menu",
                Keyboard = new ReplyKeyboardMarkup(new[]
                {
                    new[]
                    {
                        new KeyboardButton("Create document")
                    },
                    new[]
                    {
                        new KeyboardButton("My documents")
                    }
                }, true)
            };

            viewModel.SendReplyKeyboard?.Execute(replyMenu);
            return Task.CompletedTask;
        }

        protected override Task Navigation()
        {
            if (viewModel.MessageText == "Create document")
            {
                viewModel.GoToCreateDocumentPage.Execute();
            }

            if (viewModel.MessageText == "My documents")
            {
                viewModel.GoToMyDocumentsPage.Execute();
            }

            return Task.CompletedTask;
        }
    }
}