using System;
using System.Collections.Generic;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Nav;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Configuration;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Jql.Query.Order;
using QuoteFlow.Api.Jql.Values;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Search;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Jql.Builder;

namespace QuoteFlow.Core.Asset.Nav
{
    /// <summary>
    /// An <see cref="AssetTableCreator"/> that can handle normal and stable searches.
    /// 
    /// Subclasses provide the <see cref="AssetTable.Table"/> property by implementing <see cref=".GetTable()"/>.
    /// </summary>
    public class AbstractAssetTableCreator : AssetTableCreator
    {
        private readonly ApplicationProperties _applicationProperties;
		private readonly DisplayedColumnsHelper _searchColumnsFinder;
		private IDictionary<string, string> _columnSortJql;
		private SortBy _sortBy;
        private readonly IAssetTableServiceConfiguration _configuration;
		private readonly bool _fromAssetIds;
		private readonly IAssetFactory _issueFactory;
		private IList<int?> _assetIds;
		private IList<string> _issueKeys;
		private readonly ISortJqlGenerator _sortJqlGenerator;
        private IQuery _query;
		private IQuery _originalQuery;
		private readonly bool _returnIssueIds;
		private readonly ISearchHandlerManager _searchHandlerManager;
		private readonly ISearchProvider _searchProvider;
		private readonly ISearchProviderFactory _searchProviderFactory;
        private SearchRequest _searchRequest;
		internal SearchResults SearchResults;
		private readonly ISearchService _searchService;
		private readonly User _user;
		private readonly IFieldManager _fieldManager;
		private IOrderByUtil _orderByUtil;

		/// <param name="fromAssetIds"> Whether we are executing a search from issue IDs (stable update). </param>
		/// <param name="assetIds"> The requested issue IDs. </param>
		/// <param name="query"> The query whose results will form the table content. </param>
		/// <param name="returnIssueIds"> Whether issue IDs should be returned. </param>
		/// <param name="searchRequest"> The search request being executed (may differ from {@code query}). </param>
		/// <param name="user"> The user executing the search. </param>
		public AbstractAssetTableCreator(ApplicationProperties applicationProperties, 
            ColumnLayoutManager columnLayoutManager, 
            IAssetTableServiceConfiguration configuration, 
            bool fromAssetIds, 
            IAssetFactory issueFactory, 
            IList<int?> assetIds, 
            ISortJqlGenerator sortJqlGenerator, 
            IQuery query, 
            bool returnIssueIds, 
            ISearchHandlerManager searchHandlerManager, 
            ISearchProvider searchProvider, 
            ISearchProviderFactory searchProviderFactory, 
            SearchRequest searchRequest, 
            ISearchService searchService, 
            User user, 
            IFieldManager fieldManager, 
            IOrderByUtil orderByUtil)
		{
			_applicationProperties = applicationProperties;
			_configuration = configuration;
			_fromAssetIds = fromAssetIds;
			_issueFactory = issueFactory;
			_assetIds = assetIds;
			_sortJqlGenerator = sortJqlGenerator;
			_query = query;
			_returnIssueIds = returnIssueIds;
			_searchHandlerManager = searchHandlerManager;
			_searchProvider = searchProvider;
			_searchProviderFactory = searchProviderFactory;
			_searchRequest = searchRequest;
			_searchService = searchService;
			_user = user;
			_fieldManager = fieldManager;
			_orderByUtil = orderByUtil;
		    _searchColumnsFinder = new DisplayedColumnsHelper(columnLayoutManager, fieldManager);

			// query and searchRequest aren't provided as we're searching by ID. They're used
			// frequently while constructing the table, so set them here to make them available.
			if (fromAssetIds)
			{
				JqlQueryBuilder queryBuilder = GetJqlQueryBuilder();
				queryBuilder.Where().Asset().In(new MultiValueOperand(assetIds));

				IQuery stableSearchQuery = queryBuilder.BuildQuery();
			    int filterId = _searchRequest.Id;
				_searchRequest = new SearchRequest(stableSearchQuery, null, null, null, filterId);

				// Internally stable search works by performing a JQL search with the following format: issuekey in ("KEY-1", "KEY-2")
				// In order to keep consistency with the {@link SearchRequest} object, the execution of non-stable search
				// and other classes extending this class, the 'query' variable is overriden.
				// The original query is still required to generate the correct sort jql for each of the column, thus it is preserved here
				_originalQuery = query ?? stableSearchQuery;
				_query = stableSearchQuery;
			}
		}

        private JqlQueryBuilder GetJqlQueryBuilder()
        {
            return JqlQueryBuilder.NewBuilder(); // don't likes nastic staticses, do we, precious?
        }

        public override AssetTable Create()
        {
            throw new NotImplementedException();
        }
    }
}
