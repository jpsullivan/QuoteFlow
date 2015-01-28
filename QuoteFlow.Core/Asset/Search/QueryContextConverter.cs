using System.Collections.Generic;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Models;
using QuoteFlow.Core.Jql.Context;
using QuoteFlow.Core.Services;

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
                foreach (int manufacturerId in searchContext.ManufacturerIds)
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
                    foreach (int manufacturerId in searchContext.ManufacturerIds)
                    {
                        contexts.Add(new CatalogManufacturerContext(new CatalogContext(catalogId), new ManufacturerContext(manufacturerId)));
                    }
                }
            }
            
            return new QueryContext(new ClauseContext(contexts));
        }

        /// <summary>
        /// Converts a <seealso cref="IQueryContext"/> into a <seealso cref="ISearchContext"/>.
        /// If the conversion does not make sense, null is returned.
        /// 
        /// If you would like to know if this method will correctly generate a SearchContext you should call
        /// <seealso cref="SearchService.DoesQueryFitFilterForm(User, IQuery)"/>.
        /// </summary>
        /// <param name="queryContext">The QueryContext to convert into a SearchContext, if null then a null SearchContext is returned.</param>
        /// <returns>The SearchContext generated from the QueryContext, this will be null if we are unable to correctly generate a SearchContext.</returns>
        public virtual ISearchContext GetSearchContext(IQueryContext queryContext)
        {
            List<int> catalogs = null;
            List<int> manufacturers = null;

            if (queryContext != null)
            {
                // If we are unable to generate a list of catalog ids from a queryContext then we should not create a search context
                catalogs = GetSearchContextCatalogs(queryContext);
                if (catalogs != null)
                {
                    // If we are unable to generate a list of manufacturers from a queryContext then we should not create a search context
                    manufacturers = GetSearchContextManufacturers(queryContext);
                }
            }

            if (catalogs != null && manufacturers != null)
            {
                return CreateSearchContext(catalogs, manufacturers);
            }
            
            // If we couldnt generate a valid SearchContext return  null
            return null;
        }
    }
}
