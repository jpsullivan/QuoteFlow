using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using QuoteFlow.Api.Asset.Index.Indexers;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Core.Asset.Index.Indexers;
using QuoteFlow.Core.DependencyResolution;

namespace QuoteFlow.Core.Asset.Index.Managers
{
    public class FieldIndexerManager : IFieldIndexerManager
    {
        private readonly IEnumerable<IFieldIndexer> _knownIndexers;

        public IAssetSearcherManager AssetSearcherManager { get; protected set; }

        public FieldIndexerManager(IAssetSearcherManager assetSearcherManager)
        {
            AssetSearcherManager = assetSearcherManager;
            _knownIndexers = Indexers(
                typeof (CreatorIndexer),
                typeof (CreatedDateIndexer),
                typeof (DescriptionIndexer),
                typeof (AssetIdIndexer),
                typeof (AssetNameIndexer),
                typeof (AssetSkuIndexer),
                typeof (CatalogIdIndexer),
                typeof (ManufacturerIndexer),
                typeof (SummaryIndexer)
            );

            _allAssetIndexers = new Lazy<IEnumerable<IFieldIndexer>>(AllAssetIndexersFactory);
        }

        private static IEnumerable<IFieldIndexer> Indexers(params Type[] indexers)
        {
            return indexers.Select(clazz => (IFieldIndexer) Container.Kernel.TryGet(clazz)).ToList();
        }

        private readonly Lazy<IEnumerable<IFieldIndexer>> _allAssetIndexers;
        public IEnumerable<IFieldIndexer> AllAssetIndexers => _allAssetIndexers.Value; // todo: perf check

        private IEnumerable<IFieldIndexer> AllAssetIndexersFactory()
        {
            var answer = new List<IFieldIndexer>();
            var allSearchers = AssetSearcherManager.GetAllSearchers();
            foreach (var searcher in allSearchers)
            {
                answer.AddRange(searcher.SearchInformation.RelatedIndexers);
            }

            return _knownIndexers.Union(answer);
        }

        /// <summary>
        /// Not currently being used as <see cref="AllAssetIndexers"/> rebuilds the indexer list
        /// each time it is called. This is a placeholder for an eventual caching strategy that
        /// will purge the cache upon invocation.
        /// </summary>
        public void Refresh()
        {
            throw new NotImplementedException();
        }
    }
}