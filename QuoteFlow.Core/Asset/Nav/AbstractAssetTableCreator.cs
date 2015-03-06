using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Nav;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Configuration;
using QuoteFlow.Api.Infrastructure.Paging;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Jql.Query.Order;
using QuoteFlow.Api.Jql.Values;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Search;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Asset.Index;
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
		private IList<string> _assetKeys;
		private readonly ISortJqlGenerator _sortJqlGenerator;
        private IQuery _query;
		private IQuery _originalQuery;
		private readonly bool _returnAssetIds;
		private readonly ISearchHandlerManager _searchHandlerManager;
		private readonly ISearchProvider _searchProvider;
		private readonly ISearchProviderFactory _searchProviderFactory;
        private SearchRequest _searchRequest;
		private SearchResults _searchResults;
		private readonly ISearchService _searchService;
		private readonly User _user;
		private readonly IFieldManager _fieldManager;
		private IOrderByUtil _orderByUtil;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="applicationProperties"></param>
		/// <param name="columnLayoutManager"></param>
		/// <param name="configuration"></param>
        /// <param name="fromAssetIds">Whether we are executing a search from issue IDs (stable update).</param>
		/// <param name="issueFactory"></param>
        /// <param name="assetIds">The requested asset IDs.</param>
		/// <param name="sortJqlGenerator"></param>
        /// <param name="query">The query whose results will form the table content.</param>
        /// <param name="returnAssetIds">Whether asset IDs should be returned.</param>
		/// <param name="searchHandlerManager"></param>
		/// <param name="searchProvider"></param>
		/// <param name="searchProviderFactory"></param>
        /// <param name="searchRequest">The search request being executed (may differ from <see cref="query"/>.</param>
		/// <param name="searchService"></param>
        /// <param name="user">The user executing the search.</param>
		/// <param name="fieldManager"></param>
		/// <param name="orderByUtil"></param>
		public AbstractAssetTableCreator(ApplicationProperties applicationProperties, 
            ColumnLayoutManager columnLayoutManager, 
            IAssetTableServiceConfiguration configuration, 
            bool fromAssetIds, 
            IAssetFactory issueFactory, 
            IList<int?> assetIds, 
            ISortJqlGenerator sortJqlGenerator, 
            IQuery query, 
            bool returnAssetIds, 
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
			_returnAssetIds = returnAssetIds;
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

        /// <summary>
        /// Collect asset documents and IDs from Lucene.
        /// </summary>
        /// <param name="selectedAssetKey">The selected asset's key.</param>
        /// <returns>Asset documents and IDs</returns>
        private AssetDocumentAndIdCollector CollectAssets(string selectedAssetKey)
        {
            var assetSearcher = _searchProviderFactory.GetSearcher(SearchProviderTypes.AssetIndex);
            var idCollector = new AssetDocumentAndIdCollector(assetSearcher, GetStableSearchResultsLimit(),
                _configuration.NumberToShow, selectedAssetKey, _configuration.Start);

            _searchProvider.SearchAndSort(_query, _user, idCollector, new PagerFilter(0, GetStableSearchResultsLimit()));
            return idCollector;
        }

        /// <summary>
        /// Convert a list of Lucene documents to a list of assets.
        /// </summary>
        /// <param name="assetDocuments">The documents to convert.</param>
        /// <returns></returns>
        private List<IAsset> ConvertDocumentsToAssets(IList<Document> assetDocuments)
        {
            var assets = new List<IAsset>();
            foreach (var document in assetDocuments)
            {
                assets.Add(_issueFactory.GetIssue(document));
            }

            return assets;
        } 

        public override AssetTable Create()
        {
            var columnLayout = GetColumnLayout();
            var columns = GetDisplayedColumns(columnLayout);
            //var columnIds = 

            _configuration.ColumnNames = columnIds;
            if (_fromAssetIds)
            {
                ExecuteStableSearch(columns);
            }
            else
            {
                ExecuteNormalSearch(columns);
            }

            return new AssetTable()
            {
                Table = GetTable(),
                Displayed = _searchResults.Assets.Count(),
                Total = _searchResults.Total,
                StartIndex = _searchResults.Start,
                End = _searchResults.End,
                SortBy = _sortBy,
                PageSize = _configuration.NumberToShow,
                Columns = _configuration.ColumnNames,
                ColumnConfig = columnLayout.ColumnConfig,
                ColumnSortJql = _columnSortJql,
                QuoteFlowHasAssets = GetQuoteFlowHasAssets(),
                AssetIds = (_fromAssetIds ? null : _assetIds),
                AssetKeys = _assetKeys,
            };
        }

        /// <summary>
        /// Retrieve the assets matching <see cref="_query"/> and store them.
        /// If asset IDs were requested, data will also be stored in <see cref="_assetIds"/> 
        /// and <see cref="_assetKeys"/>.
        /// </summary>
        /// <param name="columns"></param>
        private void ExecuteNormalSearch(List<ColumnLayoutItem> columns)
        {
            _columnSortJql = _sortJqlGenerator.GenerateColumnSortJql(_query, GetSortableColumns(columns));
            _sortBy = _orderByUtil.GenerateSortBy(_query);

            if (_returnAssetIds)
            {
                var selectedAssetKey = _configuration.SelectedAssetKey;
                var idCollector = CollectAssets(selectedAssetKey);
                AssetDocumentAndIdCollector.Result collectedResult = idCollector.ComputeResult();

                _assetIds = collectedResult.AssetIds;
                _assetKeys = collectedResult.IssueKeys;
                _searchResults = new SearchResults(
                    ConvertDocumentsToAssets(collectedResult.Documents),
                    collectedResult.Total,
                    _configuration.NumberToShow,
                    collectedResult.Start
                    ));
            }
        }
    }
}
