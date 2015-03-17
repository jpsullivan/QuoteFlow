﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Lucene.Net.Search;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Configuration.Lucene;
using QuoteFlow.Api.Infrastructure.Collect;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Infrastructure.Lucene;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;
using QuoteFlow.Api.Util;
using WebBackgrounder;

namespace QuoteFlow.Core.Asset.Index
{
    public class AssetIndexManager : IAssetIndexManager
    {
        #region DI

        public IAssetIndexer AssetIndexer { get; protected set; }
        public IAssetService AssetService { get; protected set; }
        public IIndexPathManager IndexPathManager { get; protected set; }

        public AssetIndexManager(IAssetIndexer assetIndexer, IAssetService assetService, IIndexPathManager indexPathManager)
        {
            AssetIndexer = assetIndexer;
            AssetService = assetService;
            IndexPathManager = indexPathManager;

            _assetSearcherSupplier = new AssetSearcherSupplierImpl(manager: this);
            _commentSearcherSupplier = new CommentSearcherSupplierImpl(manager: this);
        }

        #endregion

        public global::Lucene.Net.Analysis.Analyzer AnalyzerForSearching 
        { 
            get { return QuoteFlowAnalyzer.AnalyzerForSearching; } 
        }

        public global::Lucene.Net.Analysis.Analyzer AnalyzerForIndexing
        {
            get { return QuoteFlowAnalyzer.AnalyzerForIndexing; }
        }

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

        public ICollection<string> AllIndexPaths { get; private set; }

        int ISized.Size()
        {
            throw new NotImplementedException();
        }

        int IIndexLifecycleManager.Size()
        {
            throw new NotImplementedException();
        }

        public bool IsEmpty { get; private set; }

        public int ReIndexAll(Job context)
        {
            throw new NotImplementedException();
        }

        public int ReIndexAllIssuesInBackground(Job context)
        {
            throw new NotImplementedException();
        }

        public int ReIndexAllIssuesInBackground(Job context, bool reIndexComments, bool reIndexChangeHistory)
        {
            throw new NotImplementedException();
        }

        public int Optimize()
        {
            throw new NotImplementedException();
        }

        void IIndexLifecycleManager.Shutdown()
        {
            throw new NotImplementedException();
        }

        public int Activate(Job context)
        {
            throw new NotImplementedException();
        }

        public int Activate(Job context, bool reindex)
        {
            throw new NotImplementedException();
        }

        public void Deactivate()
        {
            throw new NotImplementedException();
        }

        public bool IndexAvailable { get; private set; }
        public bool IndexConsistent { get; private set; }

        void IShutdown.Shutdown()
        {
            throw new NotImplementedException();
        }

        public int ReIndexAll()
        {
            throw new NotImplementedException();
        }

        public int ReIndexAll(Job context, bool useBackgroundReindexing, bool updateReplicatedIndexStore)
        {
            throw new NotImplementedException();
        }

        public int ReIndexAll(Job context, bool useBackgroundReindexing, bool reIndexComments, bool reIndexChangeHistory,
            bool updateReplicatedIndexStore)
        {
            throw new NotImplementedException();
        }

        public int ReIndexAssets(IEnumerable<IAsset> assets, Job context)
        {
            throw new NotImplementedException();
        }

        public int ReIndexAssets(IEnumerable<IAsset> assets, Job context, bool reIndexComments, bool reIndexChangeHistory)
        {
            throw new NotImplementedException();
        }

        public void ReIndex(IAsset issue)
        {
            throw new NotImplementedException();
        }

        public void ReIndex(IAsset issue, bool reIndexComments, bool reIndexChangeHistory)
        {
            throw new NotImplementedException();
        }

        public int ReIndexComments(IEnumerable<AssetComment> comments)
        {
            throw new NotImplementedException();
        }

        public int ReIndexComments(ICollection<AssetComment> comments, Job context)
        {
            throw new NotImplementedException();
        }

        public int ReIndexComments(ICollection<AssetComment> comments, Job context, bool updateReplicatedIndexStore)
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

        public int ReIndexAssetObjects<T>(ICollection<T> issueObjects, bool reIndexComments, bool reIndexChangeHistory) where T : IAsset
        {
            throw new NotImplementedException();
        }

        public long ReIndexAssetObjects<T>(ICollection<T> assetObjects, bool reIndexComments, bool reIndexChangeHistory, bool updateReplicatedIndexStore) where T : IAsset
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

        public IndexSearcher AssetSearcher
        {
            get
            {
                lock (IndexLocks.ReaderLock)
                {
                    return SearcherCache.ThreadLocalCache.RetrieveAssetSearcher(_assetSearcherSupplier);
                }
            }
        }

        public IndexSearcher CommentSearcher
        {
            get
            {
                lock (IndexLocks.ReaderLock)
                {
                    return SearcherCache.ThreadLocalCache.RetrieveCommentSearcher(_commentSearcherSupplier);
                }
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
    }
}