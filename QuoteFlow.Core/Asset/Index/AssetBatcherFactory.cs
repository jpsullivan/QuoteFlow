using QuoteFlow.Api.Asset;

namespace QuoteFlow.Core.Asset.Index
{
    public class AssetBatcherFactory : IAssetBatcherFactory
    {
        /// <summary>
        /// The default batch size.
        /// </summary>
        private const int BatchSize = 1000;

        private readonly IAssetFactory _assetFactory;

        public AssetBatcherFactory(IAssetFactory assetFactory)
        {
            _assetFactory = assetFactory;
        }

        public IAssetsBatcher GetBatcher()
        {
            return GetBatcher(null);
        }

        public IAssetsBatcher GetBatcher(AssetIdBatcher.ISpy spy)
        {
            return new AssetIdBatcher(_assetFactory, BatchSize, spy);
        }

        public IAssetsBatcher GetBatcher(AssetIdBatcher.ISpy spy, int batchSize)
        {
            return new AssetIdBatcher(_assetFactory, batchSize, spy);
        }
    }
}