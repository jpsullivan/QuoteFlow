namespace QuoteFlow.Models.Search.Jql.Context
{
    /// <summary>
    /// Represents an IssueType that is part of a search context.
    /// </summary>
    public interface ICatalogContext
    {
        /// <summary>
        /// The catalog id for this context element.
        /// </summary>
        int? CatalogId { get; }

        /// <summary>
        /// Indicates the special case of all catalogs that are not enumerated. If this is true then the value for
        /// catalogId will be null.
        /// </summary>
        /// <returns>True if all, false otherwise.</returns>
        bool IsAll();
    }
}