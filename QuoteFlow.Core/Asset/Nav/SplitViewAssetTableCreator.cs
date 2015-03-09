using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Configuration;
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
        public SplitViewAssetTableCreator(ApplicationProperties applicationProperties, ColumnLayoutManager columnLayoutManager, IAssetTableServiceConfiguration configuration, bool fromAssetIds, IAssetFactory issueFactory, IList<int?> assetIds, ISortJqlGenerator sortJqlGenerator, IQuery query, bool returnAssetIds, ISearchHandlerManager searchHandlerManager, ISearchProvider searchProvider, ISearchProviderFactory searchProviderFactory, SearchRequest searchRequest, ISearchService searchService, User user, IFieldManager fieldManager, IOrderByUtil orderByUtil) : base(applicationProperties, columnLayoutManager, configuration, fromAssetIds, issueFactory, assetIds, sortJqlGenerator, query, returnAssetIds, searchHandlerManager, searchProvider, searchProviderFactory, searchRequest, searchService, user, fieldManager, orderByUtil)
        {
        }

        public override object Table
        {
            get { throw new NotImplementedException(); }
        }
    }
}
