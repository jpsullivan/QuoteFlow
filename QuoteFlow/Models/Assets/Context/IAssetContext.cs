namespace QuoteFlow.Models.Assets.Context
{
    /// <summary>
    /// A context (scope) for an asset or custom field.
    /// For example, global custom fields have an AssetContext whose catalog and asset type are null.
    /// </summary>
    public interface IAssetContext
    {
        /// <summary>
        /// Gets the <see cref="Catalog"/> for this context.  A null return value is used to
        /// represent that this context applies to all catalogs.
        /// </summary>
        Catalog CatalogObject { get; }

        /// <summary>
        /// Gets the ID of the <see cref="Catalog"/> for this context. A null return value is used to
        /// represent that this context applies to all catalogs.
        /// </summary>
        int? CatalogId { get; }
    }
}