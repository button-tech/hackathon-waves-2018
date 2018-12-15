namespace WavesBot.IAL
{
    using Telegram.Bot.Types.ReplyMarkups;

    public class InlineMenu
    {
        public long MessageId { get; set; }

        public string Text { get; set; }

        public InlineKeyboardMarkup Keyboard { get; set; }
    }
}