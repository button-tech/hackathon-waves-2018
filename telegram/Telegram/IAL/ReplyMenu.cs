namespace WavesBot.IAL
{
    using Telegram.Bot.Types.ReplyMarkups;

    public class ReplyMenu
    {
        public string Text { get; set; }

        public ReplyKeyboardMarkup Keyboard { get; set; }
    }
}