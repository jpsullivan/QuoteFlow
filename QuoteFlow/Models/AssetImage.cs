namespace QuoteFlow.Models
{
    public class AssetImage
    {
        public int Id { get; set; }
        public int AssetId { get; set; }
        public string Path { get; set; }
        public string ThumbnailPath { get; set; }
        public string Caption { get; set; }
        public string Type { get; set; }
    }
}