namespace WavesBot.Data
{
    public class DocumentData
    {
        public long Id { get; set; }
        
        public long Identifier { get; set; }
        
        public string DocumentId { get; set; }
    }

    public class DocumentDto
    {
        public string DocumentId { get; set; }
    }
}