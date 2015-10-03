using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Lucene.Net.Analysis;
using Lucene.Net.Search;
using Ninject;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Configuration.Lucene;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;
using QuoteFlow.Api.Util;
using QuoteFlow.Core.DependencyResolution;
using QuoteFlow.Core.Diagnostics.Glimpse;
using QuoteFlow.Core.Index;
using QuoteFlow.Core.Lucene.Index;

namespace QuoteFlow.Core.Asset.Index
{
    public class AssetIndexManager : IAssetIndexManager
    {
        #region DI

        public IAssetIndexer AssetIndexer { get; protected set; }
        public IAssetService AssetService { get; protected set; }
        public ICatalogService CatalogService { get; protected set; }
        public IIndexPathManager IndexPathManager { get; protected set; }

        public AssetIndexManager(IAssetIndexer assetIndexer, IAssetService assetService, ICatalogService catalogService, IIndexPathManager indexPathManager)
        {
            AssetIndexer = assetIndexer;
            AssetService = assetService;
            CatalogService = catalogService;
            IndexPathManager = indexPathManager;

            _assetSearcherSupplier = new AssetSearcherSupplierImpl(manager: this);
            _commentSearcherSupplier = new CommentSearcherSupplierImpl(manager: this);
        }

        #endregion

        private static readonly object IndexReaderLock = new object();

        public Analyzer AnalyzerForSearching => QuoteFlowAnalyzer.AnalyzerForSearching;
        public Analyzer AnalyzerForIndexing => QuoteFlowAnalyzer.AnalyzerForIndexing;

        #region Asset searcher supplier / helper class

        /// <summary>
        /// Responsible for getting the actual searcher when required.
        /// </summary>
        private readonly ISupplier<IndexSearcher> _assetSearcherSupplier;

        private class AssetSearcherSupplierImpl : ISupplier<IndexSearcher>
        {
            public AssetIndexManager Manager { get; set; }

            public AssetSearcherSupplierImpl(AssetIndexManager manager)
            {
                Manager = manager;
            }

            public IndexSearcher Get()
            {
                try
                {
                    return Manager.AssetIndexer.OpenAssetSearcher();
                }
                catch (SystemException ex)
                {
                    throw ex;
                }
            }
        }

        #endregion

        #region Comment searcher supplifer / helper class

        /// <summary>
        /// Responsible for getting the actual searcher when required.
        /// </summary>
        private readonly ISupplier<IndexSearcher> _commentSearcherSupplier;

        private class CommentSearcherSupplierImpl : ISupplier<IndexSearcher>
        {
            public AssetIndexManager Manager { get; set; }

            public CommentSearcherSupplierImpl(AssetIndexManager manager)
            {
                Manager = manager;
            }

            public IndexSearcher Get()
            {
                try
                {
                    return Manager.AssetIndexer.OpenCommentSearcher();
                }
                catch (SystemException ex)
                {
                    throw ex;
                }
            }
        }

        #endregion

        public ICollection<string> AllIndexPaths
        {
            get
            {
                var paths = new List<string>();
                paths.AddRange(AssetIndexer.IndexPaths);
                return paths;
            }
        }

        public int Size()
        {
            throw new NotImplementedException();
        }

        public bool IsEmpty => Size() == 0;

        public int ReIndexAllAssetsInBackground()
        {
            return ReIndexAllAssetsInBackground(false);
        }

        public int ReIndexAllAssetsInBackground(bool reIndexComments)
        {
            return ReIndexAll(true, reIndexComments, true);
        }

        public int Optimize()
        {
            throw new NotImplementedException();
        }

        public int Activate()
        {
            throw new NotImplementedException();
        }

        public int Activate(bool reindex)
        {
            throw new NotImplementedException();
        }

        public void Deactivate()
        {
            throw new NotImplementedException();
        }

        public bool IndexAvailable
        {
            get 
            { 
                //return indexConfig.IndexAvailable; 
                throw new NotImplementedException();
            }
        }

        public bool IndexConsistent { get; private set; }

        public int ReIndexAll()
        {
            // don't use background indexing by default
            return ReIndexAll(false);
        }

        public int ReIndexAll(bool useBackgroundReindexing, bool updateReplicatedIndexStore = true)
        {
            return ReIndexAll(useBackgroundReindexing, false, updateReplicatedIndexStore);
        }

        public int ReIndexAll(bool useBackgroundReindexing, bool reIndexComments, bool updateReplicatedIndexStore)
        {
            Debug.WriteLine("Re-indexing all assets.");
            var stopWatch = Stopwatch.StartNew();

            if (useBackgroundReindexing)
            {
                using (Timeline.Capture("Reindexing all assets"))
                {
                    // doesn't delete indexes
                    lock (IndexReaderLock)
                    {
                        DoBackgroundReindex(reIndexComments);
                    }
                }

                using (Timeline.Capture("Flush thread local searchers"))
                {
                    FlushThreadLocalSearchers();
                }
            }
            else
            {
                DoStopTheWorldReindex();
            }

            stopWatch.Stop();

            return stopWatch.Elapsed.Seconds;
        }

        public int ReIndexAssets(IEnumerable<IAsset> assets)
        {
            throw new NotImplementedException();
        }

        public int ReIndexAssets(IEnumerable<IAsset> assets, bool reIndexComments)
        {
            throw new NotImplementedException();
        }

        public void ReIndex(IAsset issue)
        {
            throw new NotImplementedException();
        }

        public void ReIndex(IAsset issue, bool reIndexComments)
        {
            throw new NotImplementedException();
        }

        public int ReIndexComments(IEnumerable<AssetComment> comments)
        {
            throw new NotImplementedException();
        }

        public int ReIndexComments(ICollection<AssetComment> comments)
        {
            throw new NotImplementedException();
        }

        public int ReIndexComments(ICollection<AssetComment> comments, bool updateReplicatedIndexStore)
        {
            throw new NotImplementedException();
        }

        public void DeIndex(IAsset asset)
        {
            DeIndexAssetObjects(new HashSet<IAsset> {asset}, true);
        }

        public void DeIndexAssetObjects(ISet<IAsset> issuesToDelete, bool updateReplicatedIndexStore)
        {
            if (!issuesToDelete.AnySafe())
            {
                return;
            }

            throw new NotImplementedException();
        }

        public int ReIndexAssetObjects<T>(ICollection<T> issueObjects) where T : IAsset
        {
            throw new NotImplementedException();
        }

        public int ReIndexAssetObjects<T>(ICollection<T> issueObjects, bool reIndexComments) where T : IAsset
        {
            throw new NotImplementedException();
        }

        public long ReIndexAssetObjects<T>(ICollection<T> assetObjects, bool reIndexComments, bool updateReplicatedIndexStore) where T : IAsset
        {
            throw new NotImplementedException();
        }

        public void Hold()
        {
            throw new NotImplementedException();
        }

        public bool Held { get; private set; }

        public long Release()
        {
            throw new NotImplementedException();
        }

        public IndexSearcher GetAssetSearcher()
        {
            lock (IndexLocks.ReaderLock)
            {
                return SearcherCache.ThreadLocalCache.RetrieveAssetSearcher(_assetSearcherSupplier);
            }
        }

        public IndexSearcher GetCommentSearcher()
        {
            lock (IndexLocks.ReaderLock)
            {
                return SearcherCache.ThreadLocalCache.RetrieveCommentSearcher(_commentSearcherSupplier);
            }
        }

        public bool WithReindexLock(ThreadStart runnable)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Holds the index read/write locks.
        /// </summary>
        private class IndexLocks
        {
//            /// <summary>
//            /// Internal lock. Not to be used by clients.
//            /// </summary>
//            internal static readonly ReaderWriterLock indexLock = new ReaderWriterLock();
            internal static readonly object IndexLock = new object();
//
//            /// <summary>
//            /// The index read lock. This lock needs to be acquired when updating the index (i.e. adding to it or updating
//            /// existing documents in the index).
//            /// </summary>
//            internal IndexLock readLock = new IndexLock(indexLock.AcquireReaderLock(500));
            internal static readonly object ReaderLock = new object();
//
//            /// <summary>
//            /// The index write lock. This lock needs to be acquired only when a "stop the world" reindex is taking place and
//            /// the entire index is being deleted and re-created.
//            /// </summary>
//            internal IndexLock writeLock;
            internal static readonly object WriterLock = new object();
        }

        /// <summary>
        /// An index lock that can be acquired using a configurable time out.
        /// </summary>
        private sealed class IndexLock
        {
            private readonly ReaderWriterLock _lock;

            public IndexLock(ReaderWriterLock @lock)
            {
                _lock = @lock;
            }
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }

        AssetFactory GetAssetFactory()
        {
            return Container.Kernel.TryGet<AssetFactory>();
        }

        AssetBatcherFactory GetAssetBatcherFactory()
        {
            return Container.Kernel.TryGet<AssetBatcherFactory>();
        }

        private void DoBackgroundReindex(bool reIndexComments)
        {
            var stopWatch = Stopwatch.StartNew();

            var assetIndexHelper = new AssetIndexHelper(AssetService, AssetIndexer);
            var indexedAssets = assetIndexHelper.GetAllAssetIds();

            //var reconciler = new IndexReconciler(indexedAssets);
            var resultBuilder = new AccumulatingResultBuilder();

            Debug.WriteLine("Re-indexing {0} assets in the backgruond.", indexedAssets.Length);

//            try
//            {
//                // Index the assets one batch at a time. This stops various db drivers sucking
//                // all assets into memory at once.
//                IAssetsBatcher batcher = GetAssetBatcherFactory().GetBatcher(reconciler);
//                foreach (var batchOfAssets in batcher)
//                {
//                    resultBuilder.Add(AssetIndexer.ReIndexAssets(batchOfAssets, reIndexComments, false));
//                }
//            }
//            finally
//            {
//                
//            }

            // fuck it, lets do it live
            var allCatalogs = CatalogService.GetCatalogs(1);
            foreach (var catalog in allCatalogs)
            {
                var allAssets = AssetService.GetAssets(catalog.Id);

                using (Timeline.Capture("Reindex assets from catalog"))
                {
                    resultBuilder.Add(AssetIndexer.ReIndexAssets(allAssets, reIndexComments, false));
                }
            }

            stopWatch.Stop();
        }

        private void DoStopTheWorldReindex()
        {
            var stopWatch = Stopwatch.StartNew();

            AssetIndexer.DeleteIndexes();

            var allCatalogs = CatalogService.GetCatalogs(1);
            foreach (var catalog in allCatalogs)
            {
                var allAssets = AssetService.GetAssets(catalog.Id);
                AssetIndexer.IndexAssetsBatchMode(allAssets);
            }

            AssetIndexer.Optimize();

            stopWatch.Stop();
        }

        public static void FlushThreadLocalSearchers()
        {
            try
            {
                SearcherCache.ThreadLocalCache.CloseSearchers();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}