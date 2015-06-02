using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Models;
using QuoteFlow.Core.Jql.Context;
using QuoteFlow.Core.Services;
using Wintellect.PowerCollections;

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
        /// Converts a <see cref="IQueryContext"/> into a <see cref="ISearchContext"/>.
        /// If the conversion does not make sense, null is returned.
        /// 
        /// If you would like to know if this method will correctly generate a SearchContext you should call
        /// <see cref="SearchService.DoesQueryFitFilterForm(User, IQuery)"/>.
        /// </summary>
        /// <param name="queryContext">The QueryContext to convert into a SearchContext, if null then a null SearchContext is returned.</param>
        /// <returns>The SearchContext generated from the QueryContext, this will be null if we are unable to correctly generate a SearchContext.</returns>
        public virtual ISearchContext GetSearchContext(IQueryContext queryContext)
        {
            List<int?> catalogs = null;
            List<int?> manufacturers = null;

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
            
            // If we couldnt generate a valid SearchContext return null
            return null;
        }

        private List<int?> GetSearchContextManufacturers(IQueryContext queryContext)
        {
            var manufacturersPerCatalog = new HashSet<Set<int?>>();

            IEnumerable<QueryContextCatalogManufacturerContexts> contexts = queryContext.CatalogManufacturerContexts;

            // assume "All" manufacturer contexts return null for ids.
            foreach (QueryContextCatalogManufacturerContexts context in contexts)
            {
                var manufacturerContexts = context.ManufacturerContexts;
                var manufacturersForCatalog = new Set<int?>();
                foreach (var manufacturerContext in manufacturerContexts)
                {
                    manufacturersForCatalog.Add(manufacturerContext.ManufacturerId);
                }

                manufacturersPerCatalog.Add(manufacturersForCatalog);
            }

            var types = manufacturersPerCatalog.First();

            // If there is an "All" manufacturer context, then there can be no specific manufacturer context
            if (types.Contains(null) && types.Count != 1)
            {
                return null;
            }

            if (types.Contains(null))
            {
                return new List<int?>();
            }

            return new List<int?>(types);
        }

        private List<int?> GetSearchContextCatalogs(IQueryContext queryContext)
        {
            var catalogs = new HashSet<int?>();

            var contexts = queryContext.CatalogManufacturerContexts;

            // assume "All" type contexts return null for ids.
            foreach (QueryContextCatalogManufacturerContexts context in contexts)
            {
                int? catalogId = context.CatalogContext.CatalogId;
                catalogs.Add(catalogId);
            }

            // If there is an "All" catalog context, then there can be no specific catalog context
            if (catalogs.Contains(null) && catalogs.Count != 1)
            {
                return null;
            }

            if (catalogs.Contains(null))
            {
                return new List<int?>();
            }

            return new List<int?>(catalogs);
        }

        // We use this for testing since building a SearchContext brings up the entire world :)
        protected virtual SearchContext CreateSearchContext(List<int?> catalogs, List<int?> manufacturers)
        {
            return new SearchContext(catalogs, manufacturers);
        }
    }
}
