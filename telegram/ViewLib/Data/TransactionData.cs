namespace ViewLib.Data
{
    public class TransactionData
    {
        public string Currency { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Nickname { get; set; }
        public string FromNickName { get; set; }
        public string Value { get; set; }
        public string ValueInUsd { get; set; }
        public long FromChatId { get; set; }
        public long? ToChatId { get; set; }
    }

    public class WavesData
    {
        public string MyNickname { get; set; }
        public string MyRsaPublicKey { get; set; }

        public string PartnerNickname { get; set; }
        public string PartnerRsaPublicKey { get; set; }
    }
}