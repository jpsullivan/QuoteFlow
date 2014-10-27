namespace QuoteFlow.Models.Search.Jql.Context
{
    /// <summary>
    /// Specifies a catalog to asset types context.
    /// </summary>
    public interface ICatalogAssetTypeContext
    {
        ICatalogContext CatalogContext { get; }

        //IAssetTypeContext AssetTypeContext { get; }
    }
}