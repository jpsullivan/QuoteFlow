using System.Collections.Generic;

namespace QuoteFlow.Models.Search.Jql.Context
{
    /// <summary>
    /// Used to specify the context for an individual clause. This will be a Catalog context.
    /// </summary>
    public interface IClauseContext
    {
        /// <returns>The catalog/asset type contexts that are defined by a clause.</returns>
        HashSet<ICatalogAssetTypeContext> Contexts { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if one of the contexts is the same as CatalogContext.CreateGlobalContext().</returns>
        bool ContainsGlobalContext();
    }
}