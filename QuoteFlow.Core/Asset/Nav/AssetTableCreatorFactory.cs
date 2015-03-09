using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Nav;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Configuration;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Order;
using QuoteFlow.Api.Jql.Values;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Core.Asset.Nav
{
    /// <summary>
    /// The default implementation of <see cref="AssetTableCreatorFactory"/>.
    /// </summary>
    public class AssetTableCreatorFactory : IAssetTableCreatorFactory
    {
//        public IColumnLayoutManager ColumnLayoutManager { get; protected set; }
        public IAssetFactory AssetFactory { get; protected set; }
        public ISortJqlGenerator SortJqlGenerator { get; protected set; }
        public ISearchHandlerManager SearchHandlerManager { get; protected set; }
        public ISearchProvider SearchProvider { get; protected set; }
        public ISearchProviderFactory SearchProviderFactory { get; protected set; }
        public ISearchService SearchService { get; protected set; }
        public IFieldManager FieldManager { get; protected set; }
        public IOrderByUtil OrderByUtil { get; protected set; }
        public IDictionary<string, AbstractAssetTableCreator> AssetTableCreators { get; set; }

        public AssetTableCreatorFactory(IAssetFactory assetFactory, ISortJqlGenerator sortJqlGenerator, ISearchHandlerManager searchHandlerManager, ISearchProvider searchProvider, ISearchProviderFactory searchProviderFactory, ISearchService searchService, IFieldManager fieldManager, IOrderByUtil orderByUtil, IDictionary<string, AbstractAssetTableCreator> assetTableCreators)
        {
            AssetFactory = assetFactory;
            SortJqlGenerator = sortJqlGenerator;
            SearchHandlerManager = searchHandlerManager;
            SearchProvider = searchProvider;
            SearchProviderFactory = searchProviderFactory;
            SearchService = searchService;
            FieldManager = fieldManager;
            OrderByUtil = orderByUtil;
            AssetTableCreators.Add("split-view", SplitViewAssetTableCreator);
        }

        public AssetTableCreator GetNormalAssetTableCreator(IAssetTableServiceConfiguration configuration, IQuery query,
            bool returnAssetIds, SearchRequest searchRequest)
        {
            return CreateAssetTableCreator(configuration, false, null, query, returnAssetIds, searchRequest);
        }

        public AssetTableCreator GetStableAssetTableCreator(IAssetTableServiceConfiguration configuration, IQuery query, List<int?> assetIds,
            SearchRequest searchRequest)
        {
            return CreateAssetTableCreator(configuration, true, assetIds, query, true, searchRequest);
        }

        /// <summary>
        /// Attempt to create an <seealso cref="IssueTableCreator"/>.
        /// </summary>
        /// <returns> a new <seealso cref="IssueTableCreator"/> instance, initialised with the given arguments. </returns>
        /// <exception cref="IllegalArgumentException"> If no <seealso cref="IssueTableCreator"/> corresponds to {@code layoutKey}. </exception>
        /// <exception cref="RuntimeException"> If creating the instance fails. </exception>
        private AbstractAssetTableCreator CreateAssetTableCreator(IAssetTableServiceConfiguration configuration, bool fromIssueIds, IList<int?> assetIds, IQuery query, bool returnAssetIds, SearchRequest searchRequest)
        {
            Type clazz;
            const string errorMessage = "Creating an AssetTableCreator failed.";
            string layoutKey = configuration.LayoutKey;

            try
            {
                clazz = AssetTableCreators[layoutKey];
            }
            catch (System.NullReferenceException e)
            {
                throw new System.ArgumentException("Invalid layout key \"" + layoutKey + "\".");
            }

            if (clazz == null)
            {
                throw new System.ArgumentException("Invalid layout key \"" + layoutKey + "\".");
            }

            try
            {
                return (AbstractAssetTableCreator)clazz.GetConstructors()[0].newInstance(applicationProperties, columnLayoutManager, configuration, fromIssueIds, issueFactory, assetIds, sortJqlGenerator, query, returnAssetIds, searchHandlerManager, searchProvider, searchProviderFactory, searchRequest, searchService, user, fieldManager, orderByUtil);
            }
            catch (System.IndexOutOfRangeException e)
            {
                throw new Exception(errorMessage, e);
            }
        }

    }
}
