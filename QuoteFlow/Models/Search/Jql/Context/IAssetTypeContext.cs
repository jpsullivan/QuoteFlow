namespace QuoteFlow.Models.Search.Jql.Context
{
    /// <summary>
    /// Represents an AssetType that is part of a search context.
    /// </summary>
    public interface IAssetTypeContext
    {
        /// <summary>
        /// Returns the asset type id for this context element.
        /// </summary>
        string AssetTypeId { get; }

        /// <summary>
        /// Indicates the special case of all asset types that are not enumerated. If this is true 
        /// then the value for <see cref="AssetTypeId"/> will be null.
        /// </summary>
        /// <returns>True if all, false otherwise.</returns>
        bool All { get; }
    }
}