namespace QuoteFlow.Models.ViewModels.Quotes
{
    public class QuoteShowModel
    {
        public Quote Quote { get; set; }
        public User QuoteCreator { get; set; }

        // Dummy property to satisfy the builder viewmodel requirements
        public AssetDetailsModel CurrentAsset { get; set; }
    }
}