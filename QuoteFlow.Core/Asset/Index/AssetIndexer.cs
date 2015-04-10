using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lucene.Net.Search;
using QuoteFlow.Api.Asset;
using QuoteFlow.Api.Infrastructure.Concurrency;
using QuoteFlow.Api.Lucene.Index;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;
using QuoteFlow.Core.Index;
using QuoteFlow.Core.Lucene.Index;

namespace QuoteFlow.Core.Asset.Index
{
    public class AssetIndexer : IAssetIndexer
    {
        public IAssetDocumentFactory AssetDocumentFactory { get; protected set; }
        public static IDocumentCreationStrategy DocumentCreationStrategy { get; protected set; }

        private static Lifecycle _lifecycle;

        // simple indexing strategy just asks the operation for its result
        private readonly SimpleIndexingStrategy _simpleIndexingStrategy = new SimpleIndexingStrategy();

        public AssetIndexer(IIndexDirectoryFactory indexDirectoryFactory, 
            IAssetDocumentFactory assetDocumentFactory, IDocumentCreationStrategy documentCreationStrategy)
        {
            _lifecycle = new Lifecycle(indexDirectoryFactory);
            AssetDocumentFactory = assetDocumentFactory;
            DocumentCreationStrategy = documentCreationStrategy;
        }


        public IIndexResult IndexAssets(IEnumerable<IAsset> assets)
        {
            throw new NotImplementedException();
        }

        public IIndexResult DeIndexAssets(IEnumerable<IAsset> assets)
        {
            // As per http://stackoverflow.com/a/3894582. The IndexWriter is CPU bound, so we can try and write multiple packages in parallel.
            // The IndexWriter is thread safe and is primarily CPU-bound.
            Parallel.ForEach(assets, DeIndexAction);

            throw new NotImplementedException();
        }

        private void DeIndexAction(IAsset asset)
        {
            try
            {

            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public IIndexResult ReIndexAssets(IEnumerable<Api.Models.Asset> assets, bool reIndexComments, bool conditionalUpdate)
        {
            var operation = new ReIndexAssetsOperation(reIndexComments, conditionalUpdate);
            return Perform(assets, _simpleIndexingStrategy, operation);
        }

        private class ReIndexAssetsOperation : IIndexOperation
        {
            private readonly bool _reIndexComments;
            private readonly bool _conditionalUpdate;

            public ReIndexAssetsOperation(bool reIndexComments, bool conditionalUpdate)
            {
                _reIndexComments = reIndexComments;
                _conditionalUpdate = conditionalUpdate;
            }

            public IIndexResult Perform(IAsset asset)
            {
                try
                {
                    var mode = UpdateMode.Interactive;
                    var documents = DocumentCreationStrategy.Get(asset, _reIndexComments);
                    var assetTerm = documents.IdentifyingTerm;

                    Operation update;
                    if (_conditionalUpdate)
                    {
                        // do a conditional update using "updated" as the optimistic lock
                        update = Operations.NewConditionalUpdate(assetTerm, documents.Asset, mode, AssetFieldConstants.Updated);
                    }
                    else
                    {
                        update = Operations.NewUpdate(assetTerm, documents.Asset, mode);
                    }

                    var results = new AccumulatingResultBuilder();

                    //var onCompletion = Operations.NewCompletionDelegate()
                    var onCompletion = Operations.NewCompletionDelegate(update, null);
                    results.Add("Asset", asset.Id, _lifecycle.AssetIndex.Perform(onCompletion));

                    if (_reIndexComments)
                    {
                        results.Add("Comment for Asset", asset.Id,
                            _lifecycle.CommentIndex.Perform(Operations.NewUpdate(assetTerm, documents.Comments, mode)));
                    }

                    return results.ToResult();
                }
                catch (Exception ex)
                {
                    return new Lucene.Index.Index.Failure(ex);
                }
            }
        }

        public IIndexResult ReIndexComments(ICollection<AssetComment> comments)
        {
            throw new NotImplementedException();
        }

        public IIndexResult IndexAssetsBatchMode(IEnumerable<IAsset> assets)
        {
            throw new NotImplementedException();
        }

        public IIndexResult Optimize()
        {
            var builder = new AccumulatingResultBuilder();
            foreach (var manager in _lifecycle)
            {
                builder.Add(manager.Index.Perform(Operations.NewOptimize()));
            }
            return builder.ToResult();
        }

        public void DeleteIndexes()
        {
            throw new NotImplementedException();
        }

        public void Shutdown()
        {
            _lifecycle.Close();
        }

        public IndexSearcher OpenAssetSearcher()
        {
            return _lifecycle.Get(IndexDirectoryFactoryName.Asset).OpenSearcher();
        }

        public IndexSearcher OpenCommentSearcher()
        {
            return _lifecycle.Get(IndexDirectoryFactoryName.Comment).OpenSearcher();
        }

        public IList<string> IndexPaths
        {
            get
            {
                return new List<string>();
            }
        }

        public string IndexRootPath { get; private set; }

        /// <summary>
        /// Perform an <see cref="Operation"/> on some assets using a particular <see cref="IIndexingStrategy"/>. 
        /// There is a task context that must be updated to provide feedback to the user.
        /// The implementation needs to be thread-safe, as it may be run in parallel and maintain 
        /// a composite result to return to the caller.
        /// </summary>
        /// <param name="assets">The assets to index/deindex/reindex.</param>
        /// <param name="strategy">Single or Multi-Threaded</param>
        /// <param name="operation">Deindex/reindex/index etc.</param>
        /// <returns>The <seealso cref="IIndexResult"/> may waited on or not.</returns>
        private static IIndexResult Perform(IEnumerable<IAsset> assets, IIndexingStrategy strategy, IIndexOperation operation)
        {
            try
            {
                if (assets == null) throw new ArgumentNullException("assets");

                // thread-safe handler for the asynchronous Result
                AccumulatingResultBuilder builder = new AccumulatingResultBuilder();
                
                // perform the operation for every asset in the collection
                foreach (var asset in assets)
                {
                    var supplier = new PerformSupplier(operation, asset);
                    var result = strategy.Get(supplier);
                    builder.Add("Asset", asset.Id, result);
                }
                
                return builder.ToResult();
            }
            finally
            {
                strategy.Dispose();
            }
        }

        private class PerformSupplier : ISupplier<IIndexResult>
        {
            private readonly IIndexOperation _operation;
            private readonly IAsset _asset;

            public PerformSupplier(IIndexOperation operation, IAsset asset)
            {
                _operation = operation;
                _asset = asset;
            }

            public IIndexResult Get()
            {
                return _operation.Perform(_asset);
            }
        }

        /// <summary>
        /// Manage the life-cycle of the two index managers.
        /// </summary>
        public class Lifecycle : IEnumerable<IIndexManager>
        {
            private readonly AtomicReference<IDictionary<IndexDirectoryFactoryName, IIndexManager>> _ref = new AtomicReference<IDictionary<IndexDirectoryFactoryName, IIndexManager>>();
            private readonly IIndexDirectoryFactory _factory;

            public Lifecycle(IIndexDirectoryFactory factory)
            {
                if (factory == null) throw new ArgumentNullException("factory");
                _factory = factory;
            }

            public virtual IEnumerator<IIndexManager> GetEnumerator()
            {
                return Open().Values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            internal void Close()
            {
                IDictionary<IndexDirectoryFactoryName, IIndexManager> indexes = _ref.GetAndSet(null);
                if (indexes == null)
                {
                    return;
                }
                foreach (IIndexManager manager in indexes.Values)
                {
                    manager.Dispose();
                }
            }

            protected IDictionary<IndexDirectoryFactoryName, IIndexManager> Open()
            {
                IDictionary<IndexDirectoryFactoryName, IIndexManager> result = _ref.Value;
                while (result == null)
                {
                    //var indexFactory = _factory.Get();
                    _ref.CompareAndSet(null, _factory.Get());
                    result = _ref.Value;
                }
                return result;
            }

            internal IIndex AssetIndex
            {
                get { return Get(IndexDirectoryFactoryName.Asset).Index; }
            }

            internal IIndex CommentIndex
            {
                get { return Get(IndexDirectoryFactoryName.Comment).Index; }
            }

            public IIndexManager Get(IndexDirectoryFactoryName key)
            {
                return Open()[key];
            }

            internal IEnumerable<string> IndexPaths
            {
                get { return _factory.IndexPaths; }
            }

            internal string IndexRootPath
            {
                get { return _factory.IndexRootPath; }
            }

            internal IndexDirectoryFactoryMode Mode
            {
                set { _factory.IndexingMode = value; }
            }
        }

        /// <summary>
        /// An <see cref="IIndexOperation"/> performs the actual update to the index for a 
        /// specific <see cref="IAsset"/>.
        /// </summary>
        private interface IIndexOperation
        {
            IIndexResult Perform(IAsset asset);
        }
    }
}