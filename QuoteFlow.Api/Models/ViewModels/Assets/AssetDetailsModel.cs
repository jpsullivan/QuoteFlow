namespace QuoteFlow.Api.Models.ViewModels.Assets
{
    public class AssetDetailsModel
    {
        public Asset Asset { get; set; }
        public Catalog Catalog { get; set; }
        public bool BuilderEnabled { get; set; }

        public AssetDetailsModel() { }

        public AssetDetailsModel(Asset asset, bool builderEnabled)
        {
            Asset = asset;
            BuilderEnabled = builderEnabled;
        }
    }
}
