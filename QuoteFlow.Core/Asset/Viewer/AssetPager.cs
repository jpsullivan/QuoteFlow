using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Asset.Viewer
{
    /// <summary>
    /// Contains asset pager information: the next/previous assets.
    /// </summary>
    public class AssetPager
    {
        public class AssetDescriptor
        {
            public int Id { get; set; }
            public string Sku { get; set; }

            public AssetDescriptor(IAsset asset)
            {
                Id = asset.Id;
                Sku = asset.SKU;
            }
        }

        public AssetDescriptor PreviousAsset { get; set; }
        public AssetDescriptor NextAsset { get; set; }
        public int Position { get; set; }
        public int ResultCount { get; set; }

        public AssetPager(IAsset nextAsset, int position, IAsset previousAsset, int resultCount)
        {
            Position = position;
            ResultCount = resultCount;

            if (nextAsset != null)
            {
                NextAsset = new AssetDescriptor(nextAsset);
            }

            if (previousAsset != null)
            {
                PreviousAsset = new AssetDescriptor(previousAsset);
            }
        }
    }
}