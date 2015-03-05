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

namespace QuoteFlow.Api.Asset.Nav
{
    /// <summary>
    /// The default implementation of <see cref="AssetTableCreatorFactory"/>.
    /// </summary>
    public class AssetTableCreatorFactory : IAssetTableCreatorFactory
    {
        public IColumnLayoutManager ColumnLayoutManager { get; protected set; }
        public IAssetFactory AssetFactory { get; protected set; }
        public ISortJqlGenerator SortJqlGenerator { get; protected set; }
        public ISearchHandlerManager SearchHandlerManager { get; protected set; }
        public ISearchProvider SearchProvider { get; protected set; }
        public ISearchProviderFactory SearchProviderFactory { get; protected set; }
        public ISearchService SearchService { get; protected set; }
        public IFieldManager FieldManager { get; protected set; }
        public IOrderByUtil OrderByUtil { get; protected set; }

        private IDictionary<string, AbstractAssetTableCreator> assetTableCreators;

        public AssetTableCreatorFactory(IColumnLayoutManager columnLayoutManager, IAssetFactory assetFactory, ISortJqlGenerator sortJqlGenerator, ISearchHandlerManager searchHandlerManager, ISearchProvider searchProvider, ISearchProviderFactory searchProviderFactory, ISearchService searchService, IFieldManager fieldManager, IOrderByUtil orderByUtil)
        {
            ColumnLayoutManager = columnLayoutManager;
            AssetFactory = assetFactory;
            SortJqlGenerator = sortJqlGenerator;
            SearchHandlerManager = searchHandlerManager;
            SearchProvider = searchProvider;
            SearchProviderFactory = searchProviderFactory;
            SearchService = searchService;
            FieldManager = fieldManager;
            OrderByUtil = orderByUtil;
        }


        public AssetTableCreator GetNormalAssetTableCreator(IAssetTableServiceConfiguration configuration, IQuery query,
            bool returnAssetIds, SearchRequest searchRequest, User user)
        {
            throw new NotImplementedException();
        }

        public AssetTableCreator GetStableAssetTableCreator(IAssetTableServiceConfiguration configuration, IQuery query, List<int?> assetIds,
            SearchRequest searchRequest, User user)
        {
            throw new NotImplementedException();
        }
    }
}
