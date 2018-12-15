namespace ViewLib.Data
{
    public class TokenData
    {
        public string Token { get; set; }
        public string TokenAddress { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Nickname { get; set; }
        public string Value { get; set; }
        public string ValueInUsd { get; set; }
        public long FromChatId { get; set; }
        public long? ToChatId { get; set; }

        public string FromNickName { get; set; }
    }
}