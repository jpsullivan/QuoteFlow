using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Asset;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Configuration;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Order;
using QuoteFlow.Api.Jql.Values;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Core.Asset.Nav
{
    /// <summary>
    /// The split-view asset table.
    /// </summary>
    public class SplitViewAssetTableCreator : AbstractAssetTableCreator
    {
        public SplitViewAssetTableCreator(IAssetTableServiceConfiguration configuration, 
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
            IOrderByUtil orderByUtil,
            ICatalogService catalogService, // todo remove these after lucene 4.8 upgrade
            IAssetService assetService) 
            : base(configuration, fromAssetIds, issueFactory, assetIds, sortJqlGenerator, query, returnAssetIds, searchHandlerManager, searchProvider, searchProviderFactory, searchRequest, searchService, user, fieldManager, orderByUtil, catalogService, assetService)
        {
        }

        protected override IEnumerable<AssetTableRow> GetTable()
        {
            var tableRows = SearchResults.Assets.Select(asset => asset.ToTableRow()).ToList();
            return tableRows;
        }
    }
}
