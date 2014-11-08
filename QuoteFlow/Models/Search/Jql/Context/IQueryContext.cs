using System.Collections.Generic;

namespace QuoteFlow.Models.Search.Jql.Context
{
    /// <summary>
    /// Represents a context of projects and issuetypes generated from a search query.
    /// </summary>
    public interface IQueryContext
    {
        IEnumerable<CatalogAssetTypeContexts> CatalogAssetTypeContexts { get; set; }
    }
}