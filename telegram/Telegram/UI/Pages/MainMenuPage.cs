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
                Text = "Главное меню",
                Keyboard = new ReplyKeyboardMarkup(new[]
                {
                    new[]
                    {
                        new KeyboardButton("Создать документ")
                    },
                    new[]
                    {
                        new KeyboardButton("Мои документы")
                    }
                }, true)
            };

            viewModel.SendReplyKeyboard?.Execute(replyMenu);
            return Task.CompletedTask;
        }

        protected override Task Navigation()
        {
            if (viewModel.MessageText == "Создать документ")
            {
                viewModel.GoToCreateDocumentPage.Execute();
            }

            if (viewModel.MessageText == "Мои документы")
            {
                viewModel.GoToMyDocumentsPage.Execute();
            }

            return Task.CompletedTask;
        }
    }
}