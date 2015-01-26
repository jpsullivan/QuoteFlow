using QuoteFlow.Api.Models.ViewModels.Assets;

namespace QuoteFlow.Api.Models.ViewModels.Quotes
{
    public class QuoteShowModel
    {
        public Quote Quote { get; set; }
        public User QuoteCreator { get; set; }

        // Dummy property to satisfy the builder viewmodel requirements
        public AssetDetailsModel CurrentAsset { get; set; }
    }
}