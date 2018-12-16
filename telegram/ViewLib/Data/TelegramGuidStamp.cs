namespace ViewLib.Data
{
    using System;

    public class TelegramGuidStamp
    {
        public long Identifier { get; set; }

        public string NickName { get; set; }

        public string Guid { get; set; }

        public WavesData WavesData { get; set; }

        public TokenData TokenData { get; set; }

        public DateTimeOffset LifetimeToDelete { get; set; }
    }
}