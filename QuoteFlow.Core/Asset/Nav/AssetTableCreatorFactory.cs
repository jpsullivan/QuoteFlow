using System;
using System.Collections.Generic;
using QuoteFlow.Api.Asset;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Nav;
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
    /// The default implementation of <see cref="AssetTableCreatorFactory"/>.
    /// </summary>
    public class AssetTableCreatorFactory : IAssetTableCreatorFactory
    {
        public IAssetFactory AssetFactory { get; protected set; }
        public ISortJqlGenerator SortJqlGenerator { get; protected set; }
        public ISearchHandlerManager SearchHandlerManager { get; protected set; }
        public ISearchProvider SearchProvider { get; protected set; }
        public ISearchProviderFactory SearchProviderFactory { get; protected set; }
        public ISearchService SearchService { get; protected set; }
        public IFieldManager FieldManager { get; protected set; }
        public IOrderByUtil OrderByUtil { get; protected set; }
        public IDictionary<string, Type> AssetTableCreators { get; set; }

        public ICatalogService CatalogService { get; set; }
        public IAssetService AssetService { get; set; }

        public AssetTableCreatorFactory(IAssetFactory assetFactory, ISortJqlGenerator sortJqlGenerator, ISearchHandlerManager searchHandlerManager, ISearchProvider searchProvider, ISearchProviderFactory searchProviderFactory, ISearchService searchService, IFieldManager fieldManager, IOrderByUtil orderByUtil, ICatalogService catalogService, IAssetService assetService)
        {
            AssetFactory = assetFactory;
            SortJqlGenerator = sortJqlGenerator;
            SearchHandlerManager = searchHandlerManager;
            SearchProvider = searchProvider;
            SearchProviderFactory = searchProviderFactory;
            SearchService = searchService;
            FieldManager = fieldManager;
            OrderByUtil = orderByUtil;

            // todo remove these after lucene 4.8 upgrade
            CatalogService = catalogService;
            AssetService = assetService;

            AssetTableCreators = new Dictionary<string, Type>
            {
                {"split-view", typeof (SplitViewAssetTableCreator)}
            };
        }

        public AssetTableCreator GetNormalAssetTableCreator(User user, IAssetTableServiceConfiguration configuration, IQuery query,
            bool returnAssetIds, SearchRequest searchRequest)
        {
            return CreateAssetTableCreator(user, configuration, false, null, query, returnAssetIds, searchRequest);
        }

        public AssetTableCreator GetStableAssetTableCreator(User user, IAssetTableServiceConfiguration configuration, IQuery query, List<int?> assetIds,
            SearchRequest searchRequest)
        {
            return CreateAssetTableCreator(user, configuration, true, assetIds, query, true, searchRequest);
        }

        /// <summary>
        /// Attempt to create an <see cref="AssetTableCreator"/>.
        /// </summary>
        /// <returns>A new <see cref="AssetTableCreator"/> instance, initialised with the given arguments.</returns>
        /// <exception cref="IllegalArgumentException"> If no <see cref="AssetTableCreator"/> corresponds to {@code layoutKey}. </exception>
        /// <exception cref="RuntimeException"> If creating the instance fails. </exception>
        private AbstractAssetTableCreator CreateAssetTableCreator(User user, IAssetTableServiceConfiguration configuration, bool fromIssueIds, IList<int?> assetIds, IQuery query, bool returnAssetIds, SearchRequest searchRequest)
        {
            Type clazz;
            const string errorMessage = "Creating an AssetTableCreator failed.";
            string layoutKey = configuration.LayoutKey;

            try
            {
                clazz = AssetTableCreators[layoutKey];
            }
            catch (NullReferenceException e)
            {
                throw new ArgumentException("Invalid layout key \"" + layoutKey + "\".");
            }

            if (clazz == null)
            {
                throw new ArgumentException("Invalid layout key \"" + layoutKey + "\".");
            }

            try
            {
                // passing ctor params in the most ghetto way possible
                var args = new
                {
                    configuration,
                    fromIssueIds,
                    AssetFactory,
                    assetIds,
                    SortJqlGenerator,
                    query,
                    returnAssetIds,
                    SearchHandlerManager,
                    SearchProvider,
                    SearchProviderFactory,
                    searchRequest,
                    SearchService,
                    user,
                    FieldManager,
                    OrderByUtil,
                    CatalogService, // todo remove these after lucene 4.8 upgrade
                    AssetService
                };

                return (AbstractAssetTableCreator)Activator.CreateInstance(clazz, 
                    configuration,
                    fromIssueIds,
                    AssetFactory,
                    assetIds,
                    SortJqlGenerator,
                    query,
                    returnAssetIds,
                    SearchHandlerManager,
                    SearchProvider,
                    SearchProviderFactory,
                    searchRequest,
                    SearchService,
                    user,
                    FieldManager,
                    OrderByUtil,
                    CatalogService,
                    AssetService);
            }
            catch (IndexOutOfRangeException e)
            {
                throw new Exception(errorMessage, e);
            }
        }

    }
}
