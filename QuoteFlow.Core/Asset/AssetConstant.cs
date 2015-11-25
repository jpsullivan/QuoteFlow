using QuoteFlow.Api.Asset;

namespace QuoteFlow.Core.Asset
{
    public class AssetConstant : IAssetConstant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public string IconUrlHtml { get; set; }
    }
}