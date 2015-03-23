namespace QuoteFlow.Core.Asset.Index
{
    public interface IAssetBatcherFactory
    {
        IAssetsBatcher GetBatcher();

        IAssetsBatcher GetBatcher(AssetIdBatcher.ISpy spy);

        IAssetsBatcher GetBatcher(AssetIdBatcher.ISpy spy, int batchSize);
    }
}