namespace ViewLib.Data
{
    using System;

    public class TelegramGuidStamp
    {
        public long ChatId { get; set; }

        public int UserID { get; set; }

        public string NickName { get; set; }

        public string Guid { get; set; }

        public TransactionData TransactionData { get; set; }

        public TokenData TokenData { get; set; }

        public DateTimeOffset LifetimeToDelete { get; set; }
    }
}