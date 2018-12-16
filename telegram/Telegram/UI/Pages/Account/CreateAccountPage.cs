namespace WavesBot.UI.Pages.Account
{
    using System.Threading.Tasks;
    using IAL;
    using Telegram.Bot.Types.ReplyMarkups;
    using ViewModels.Account;

    public class CreateAccountPage : BaseTelegramPage
    {
        private readonly CreateAccountViewModel viewModel;

        public CreateAccountPage(CreateAccountViewModel viewModel) : base(viewModel)
        {
            this.viewModel = viewModel;
            IsAlwaysDefaultRender = true;
        }

        protected override async Task Default()
        {
            var exist = await viewModel.CheckAccountExist();
            if (!exist)
            {
                var createUrl = await viewModel.GetCreateUrl();
                var inlineMenu = new InlineMenu
                {
                    Text = "Register",
                    Keyboard =
                        new InlineKeyboardMarkup(new[]
                        {
                            new[]
                            {
                                new InlineKeyboardButton
                                {
                                    Text = "Create account",
                                    CallbackData = CallBack.RootPage.ToString(),
                                    Url = createUrl
                                }
                            }
                        })
                };
                viewModel.SendInlineKeyboard?.Execute(inlineMenu);
            }
            else
            {
                viewModel.RemoteState.Execute();
            }
        }
    }
}