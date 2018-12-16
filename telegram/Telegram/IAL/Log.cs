namespace WavesBot.IAL
{
    using System;

    public class Log
    {
        public LogTypes Type { get; set; }

        public string Message { get; set; }

        public LogFromTypes LogFromType { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
    }

    public enum LogTypes
    {
        Undefined = 0,
        Exception = 1,
        Warning = 2
    }

    public enum LogFromTypes
    {
        TelegramBot = 0,
        Front = 1
    }
}