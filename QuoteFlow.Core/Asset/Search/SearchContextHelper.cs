using System.Collections.Generic;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Asset.Transport;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Core.Asset.Search
{
    /// <summary>
    /// Default implementation of ISearchContextHelper.
    /// </summary>
    public class SearchContextHelper : ISearchContextHelper
    {
        #region DI

        public ISearchService SearchService { get; protected set; }
        public IAssetSearcherManager AssetSearcherManager { get; protected set; }

        public SearchContextHelper(ISearchService searchService, IAssetSearcherManager assetSearcherManager)
        {
            SearchService = searchService;
            AssetSearcherManager = assetSearcherManager;
        }

        #endregion

        public SearchContextWithFieldValues GetSearchContextWithFieldValuesFromJqlString(string query)
        {
            if (query.HasValue())
            {
                var jqlQuery = SearchService.ParseQuery(null, query);
                if (jqlQuery.IsValid())
                {
                    var searchContext = SearchService.GetSearchContext(null, jqlQuery.Query);
                    return GetSearchContextWithFieldValuesFromQuery(searchContext, jqlQuery.Query);
                }
            }

            return new SearchContextWithFieldValues(CreateSearchContext(), new FieldValuesHolder());
        }

        public ISearchContext GetSearchContextFromJqlString(string query)
        {
            if (query.HasValue())
            {
                var jqlQuery = SearchService.ParseQuery(null, query);
                if (jqlQuery.IsValid())
                {
                    return SearchService.GetSearchContext(null, jqlQuery.Query);
                }
            }

            return CreateSearchContext();
        }

        public SearchContextWithFieldValues GetSearchContextWithFieldValuesFromQuery(ISearchContext searchContext, IQuery query)
        {
            var fieldValuesHolder = CreateFieldValuesHolderFromQuery(query, searchContext);
            return new SearchContextWithFieldValues(searchContext, fieldValuesHolder);
        }

        private IFieldValuesHolder CreateFieldValuesHolderFromQuery(IQuery query, ISearchContext searchContext)
        {
            var fieldValuesHolder = new FieldValuesHolder();
            foreach (var searcher in AssetSearcherManager.GetAllSearchers())
            {
                searcher.SearchInputTransformer.PopulateFromQuery(null, fieldValuesHolder, query, searchContext);
            }
            return fieldValuesHolder;
        }

        private static ISearchContext CreateSearchContext()
        {
            return new SearchContext(new List<int?>(), null);
        }
    }
}