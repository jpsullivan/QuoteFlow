using System.Collections.Generic;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Core.Jql.Context;

namespace QuoteFlow.Core.Asset.Search
{
    /// <summary>
    /// A utlility class for converting a <see cref="QueryContext"/> into a <see cref="SearchContext"/>.
    /// This conversion only makes sense if the <see cref="IQuery"/> the QueryContext was generated from fits the simple navigator.
    /// </summary>
    public class QueryContextConverter
    {
        /// <summary>
        /// Converts a <see cref="SearchContext"/> representation into the 
        /// <see cref="QueryContext"/> of a search context.
        /// 
        /// As search contexts represented by <see cref="QueryContext"/>s is a super set of those
        /// represented by <see cref="SearchContext"/>, this coversion will always be valid and
        /// never return null.
        /// </summary>
        /// <param name="searchContext">The context to convert into a <see cref="QueryContext"/> </param>
        /// <returns>The context represented by a <see cref="QueryContext"/>. Never Null. </returns>
        public virtual QueryContext GetQueryContext(ISearchContext searchContext)
        {
            var contexts = new HashSet<ICatalogManufacturerContext>();
            
            if (searchContext.IsForAnyCatalogs() && searchContext.IsForAnyManufacturers())
            {
                return new QueryContext(ClauseContext.CreateGlobalClauseContext());
            }
            
            if (searchContext.IsForAnyCatalogs())
            {
                foreach (int manufacturerId in searchContext.IssueTypeIds)
                {
                    contexts.Add(new CatalogManufacturerContext(AllCatalogsContext.Instance, new ManufacturerContext(manufacturerId)));
                }
            }
            else if (searchContext.IsForAnyManufacturers())
            {
                foreach (int catalogId in searchContext.CatalogIds)
                {
                    contexts.Add(new CatalogManufacturerContext(new CatalogContext(catalogId), AllManufacturersContext.Instance));
                }
            }
            else
            {
                foreach (int catalogId in searchContext.CatalogIds)
                {
                    foreach (int manufacturerId in searchContext.IssueTypeIds)
                    {
                        contexts.Add(new CatalogManufacturerContext(new CatalogContext(catalogId), new ManufacturerContext(manufacturerId)));
                    }
                }
            }
            
            return new QueryContext(new ClauseContext(contexts));
        }
    }
}
