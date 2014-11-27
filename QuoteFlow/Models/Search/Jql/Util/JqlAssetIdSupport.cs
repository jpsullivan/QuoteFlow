namespace QuoteFlow.Models.Search.Jql.Util
{
    /// <summary>
    /// Default implementation of the <see cref="IJqlAssetIdSupport"/> interface.
    /// </summary>
    public class JqlAssetIdSupport : IJqlAssetIdSupport
    {
        public bool IsValidAssetId(int assetId)
        {
            return assetId > 0;
        }
    }
}