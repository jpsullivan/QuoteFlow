using System.Collections.Generic;
using QuoteFlow.Api.Asset.Context;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Asset.Search
{
    /// <summary>
    /// Represents the Project and IssueType combination which is used to determine the allowable fields and values
    /// when searching for Issues. In QuoteFlow Schemes generally define what is customised on a per "Context" basis.
    /// </summary>
    public interface ISearchContext
    {
        /// <summary>
        /// Returns whether the context is <em>global</em> or not. A context is global when there 
        /// are no catalog restrictions.
        /// </summary>
        bool IsForAnyCatalogs();

        /// <summary>
        /// Returns true if there is exactly one Catalog in this SearchContext.
        /// </summary>
        bool IsSingleCatalogContext();

        /// <summary>
        /// Returns the single <see cref="Catalog"/> for this SearchContext.
        /// </summary>
        Catalog SingleCatalog { get; }

        /// <summary>
        /// All of the catalog ID's for this context.
        /// </summary>
        List<int> CatalogIds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        IEnumerable<IAssetContext> AsAssetContexts { get; }

        /// <summary>
        /// Verifies that all asset types and catalogs in the search context actually still exists.
        /// This might not be the case. Also removes any catalogs or asset types from this SearchContext
        /// that do not (any longer) exist in the backing store.
        /// </summary>
        void Verify();

        /// <summary>
        /// Returns <see cref="Catalog"/> objects in this search context.
        /// </summary>
        IEnumerable<Catalog> Catalogs { get; set; }
    }
}