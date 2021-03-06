namespace WavesBot.UI.Pages.Documents
{
    using System.Threading.Tasks;
    using ViewModels.Documents;

    public class ChoicePartnerNicknamePage : BaseTelegramPage
    {
        private readonly ChoicePartnerNicknameViewModel viewModel;

        public ChoicePartnerNicknamePage(ChoicePartnerNicknameViewModel viewModel) : base(viewModel)
        {
            this.viewModel = viewModel;
        }

        protected override Task Message()
        {
            if (viewModel.MessageText == "Create document")
                return Task.CompletedTask;
            
            viewModel.SetUserPartnerNick(viewModel.MessageText?.Replace("@", ""));
            viewModel.GoToCreateDocument.Execute();

            return Task.CompletedTask;
        }

        protected override Task Default()
        {
            var text = "Enter partner nickname (@nickname)";
            viewModel.SendMessage.Execute(text);
            return base.Default();
        }
    }
}