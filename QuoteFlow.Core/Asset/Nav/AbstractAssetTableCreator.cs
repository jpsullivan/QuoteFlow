using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using QuoteFlow.Api.Asset;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Fields.Layout.Column;
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
using QuoteFlow.Api.Util;
using QuoteFlow.Core.Asset.Index;
using QuoteFlow.Core.Jql.Builder;

namespace QuoteFlow.Core.Asset.Nav
{
    /// <summary>
    /// An <see cref="AssetTableCreator"/> that can handle normal and stable searches.
    /// 
    /// Subclasses provide the <see cref="AssetTable.Table"/> property by implementing <see cref=".GetTable()"/>.
    /// </summary>
    public abstract class AbstractAssetTableCreator : AssetTableCreator
    {
//        private readonly ApplicationProperties _applicationProperties;
//		private readonly DisplayedColumnsHelper _searchColumnsFinder;
		private IDictionary<string, string> _columnSortJql;
		private SortBy _sortBy;
        private readonly IAssetTableServiceConfiguration _configuration;
		private readonly bool _fromAssetIds;
		private readonly IAssetFactory _assetFactory;
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
		public SearchResults SearchResults;
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
		/// <param name="assetFactory"></param>
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
		public AbstractAssetTableCreator(IAssetTableServiceConfiguration configuration, 
            bool fromAssetIds, 
            IAssetFactory assetFactory, 
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
			_configuration = configuration;
			_fromAssetIds = fromAssetIds;
			_assetFactory = assetFactory;
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
            var idCollector = new AssetDocumentAndIdCollector(assetSearcher, StableSearchResultsLimit,
                _configuration.NumberToShow, selectedAssetKey, _configuration.Start);

            _searchProvider.SearchAndSort(_query, _user, idCollector, new PagerFilter<object>(0, StableSearchResultsLimit));
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
                assets.Add(_assetFactory.GetAsset(document));
            }

            return assets;
        } 

        public override AssetTable Create()
        {
//            var columnLayout = ColumnLayout;
//            var columns = GetDisplayedColumns(columnLayout).ToList();
//            var columnIds = columns.Select(c => c.Id).ToList();

            //_configuration.ColumnNames = columnIds;
            if (_fromAssetIds)
            {
                ExecuteStableSearch();
            }
            else
            {
                ExecuteNormalSearch();
            }

            return new AssetTable()
            {
                Table = Table,
                Displayed = SearchResults.Assets.Count(),
                Total = SearchResults.Total,
                StartIndex = SearchResults.Start,
                End = SearchResults.End,
                SortBy = _sortBy,
                PageSize = _configuration.NumberToShow,
                Columns = _configuration.ColumnNames,
                ColumnConfig = "USER",
                ColumnSortJql = _columnSortJql,
                QuoteFlowHasAssets = QuoteFlowHasAssets,
                AssetIds = (_fromAssetIds ? null : _assetIds),
                AssetKeys = _assetKeys,
            };
        }

        /// <summary>
        /// Retrieve the assets matching <see cref="_query"/> and store them.
        /// If asset IDs were requested, data will also be stored in <see cref="_assetIds"/> 
        /// and <see cref="_assetKeys"/>.
        /// </summary>
        private void ExecuteNormalSearch()
        {
            //_columnSortJql = _sortJqlGenerator.GenerateColumnSortJql(_query, GetSortableColumns(columns));
            _sortBy = _orderByUtil.GenerateSortBy(_query);

            if (_returnAssetIds)
            {
                var selectedAssetKey = _configuration.SelectedAssetKey;
                var idCollector = CollectAssets(selectedAssetKey);
                AssetDocumentAndIdCollector.Result collectedResult = idCollector.ComputeResult();

                _assetIds = collectedResult.AssetIds;
                _assetKeys = collectedResult.IssueKeys;
                SearchResults = new SearchResults(
                    ConvertDocumentsToAssets(collectedResult.Documents),
                    collectedResult.Total,
                    _configuration.NumberToShow,
                    collectedResult.Start
                    );
            }
            else
            {
                var pagerFilter = new PagerFilter<object>(_configuration.Start, _configuration.NumberToShow);
                SearchResults = _searchProvider.Search(_query, _user, pagerFilter);

                // Ensure that the start index doesn't exceed the number of results
                int pageSize = pagerFilter.PageSize;
                int resultsCount = SearchResults.Total;
                while (pagerFilter.Start > 0 && pagerFilter.Start >= SearchResults.Total)
                {
                    pagerFilter.Start = Math.Max(0, resultsCount - 1) / pageSize * pageSize;
                    SearchResults = _searchProvider.Search(_query, _user, pagerFilter);
                }
            }
        }

        /// <summary>
        /// Retrieve and store the assets whose IDs were passed to the constructor from the Lucene index.
        /// The results are stored in <see cref="SearchResults"/> and they are stored as they were
        /// in the ID list.
        /// </summary>
        private void ExecuteStableSearch()
        {
            //_columnSortJql = _sortJqlGenerator.GenerateColumnSortJql(_originalQuery, GetSortableColumns(columns));

            var pagerFilter = new PagerFilter<object>(0, StableSearchResultsLimit);
            SearchResults = _searchProvider.Search(_query, _user, pagerFilter);

            // Sort results so they are in the same order as the list of asset IDs
            var assetMap = SearchResults.Assets.ToDictionary(asset => asset.Id);
            var sortedAssets = _assetIds.Select(assetId => assetMap[(int) assetId]).ToList();

            SearchResults = new SearchResults(sortedAssets, SearchResults.Total, pagerFilter);
        }

        /// <summary>
        /// Returns the layout items for requested columns </summary>
        /// <param name="columnNames"> of requested columns
        /// </param>
        /// <returns> the columns that should be visible to the user. </returns>
        private IList<IColumnLayoutItem> GetDisplayedColumns(IList<string> columnNames)
        {
            return new List<IColumnLayoutItem>();
            //return _searchColumnsFinder.GetDisplayedColumns(_user, columnNames).ColumnLayoutItems;
        }

        /// <summary>
        /// Returns the layout items for a given columnLayout </summary>
        /// <param name="columns"> layout </param>
        /// <returns> the columns that should be visible to the user. </returns>
        private IEnumerable<IColumnLayoutItem> GetDisplayedColumns(IColumnLayout columns)
        {
            return columns.ColumnLayoutItems;
        }

        /// <summary>
        /// Returns the columns that are displayed to the user in list view.
        /// </summary>
        /// <returns> the columns that should be visible to the user. </returns>
        private IColumnLayout ColumnLayout
        {
            get
            {
                return null;
                //return _searchColumnsFinder.GetDisplayedColumns(_user, _searchRequest, _configuration);
            }
        }

        /// <returns> whether the JIRA instance contains any issues. </returns>
        /// <exception cref="SearchException"> If querying the Lucene index fails. </exception>
        private bool QuoteFlowHasAssets
        {
            get
            {
                if (SearchResults.Total > 0)
                {
                    return true;
                }
                
                return _searchProvider.SearchCountOverrideSecurity(new Query(), _user) > 0;
            }
        }

        /// <summary>
        /// Returns the maximum number of assets in a stable search.
        /// </summary>
        private int StableSearchResultsLimit
        {
            get { return 50; }
        }

        /// <summary>
        /// Calculate the issue table's start index, taking the selected issue into consideration.
        /// </summary>
        /// <param name="issueKeys"> The keys of all issues matching the search. </param>
        /// <returns> The index of the first issue to show in the table. </returns>
        private int GetStartIndex(IList<string> issueKeys)
        {
            int pageSize = _configuration.NumberToShow;
            string selectedAssetKey = _configuration.SelectedAssetKey;

            if (selectedAssetKey != null)
            {
                return Math.Max(issueKeys.IndexOf(selectedAssetKey), 0) / pageSize * pageSize;
            }
            
            return _configuration.Start;
        }

        public abstract object Table { get; }

        /// <summary>
        /// Validate the issue table request.
        /// </summary>
        /// <returns> a message set containing any validation errors/warnings. </returns>
        public override IMessageSet Validate()
        {
            // Don't run validation stable search query (we assume its correct).
            if (!_fromAssetIds)
            {
                int filterId = _searchRequest != null ? _searchRequest.Id : 0;
                return _searchService.ValidateQuery(_user, _query, filterId);
            }

            return new MessageSet();
        }

        /// <summary>
        /// Returns the fields for which we display sortable columns on the issue navigator. We generate sort JQL for these
        /// columns so that when the user clicks them that instantly changes the sort.
        /// </summary>
        /// <param name="displayedColumns"> list of columns to be displayed.
        /// </param>
        /// <returns> the columns to generate sort JQL for. </returns>
        private IEnumerable<INavigableField> GetSortableColumns(IEnumerable<IColumnLayoutItem> displayedColumns)
        {
            // add the columns that are displayed in list view
            var fields = displayedColumns.Select(columnLayoutItem => columnLayoutItem.NavigableField).ToList();

            // add the columns that are in the ORDER BY clause
            fields.AddRange(OrderByFields);

            return fields;
        }

        /// <summary>
        /// Returns the NavigableField instances for the fields present in the query's ORDER BY clause.
        /// </summary>
        /// <returns> NavigableField instances for the fields present in the query's ORDER BY clause. </returns>
        private IList<INavigableField> OrderByFields
        {
            get
            {
                IOrderBy orderBy = _query.OrderByClause;
                if (orderBy == null || orderBy.SearchSorts == null || !orderBy.SearchSorts.Any())
                {
                    return new List<INavigableField>();
                }

                var orderByFields = new List<INavigableField>();
                foreach (SearchSort sort in orderBy.SearchSorts)
                {
                    ICollection<string> fieldIds = _searchHandlerManager.GetFieldIds(sort.Field);
                    orderByFields.AddRange(fieldIds.Select(fieldId => _fieldManager.GetField(fieldId)).OfType<INavigableField>());
                }

                return orderByFields;
            }
        }

    }
}
