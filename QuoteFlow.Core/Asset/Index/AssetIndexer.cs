using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using QuoteFlow.Api.Asset;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Infrastructure.Concurrency;
using QuoteFlow.Api.Lucene.Index;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;
using QuoteFlow.Core.Index;
using QuoteFlow.Core.Lucene.Index;
using Timeline = QuoteFlow.Core.Diagnostics.Glimpse.Timeline;

namespace QuoteFlow.Core.Asset.Index
{
    public class AssetIndexer : IAssetIndexer
    {
        public IAssetDocumentFactory AssetDocumentFactory { get; protected set; }

        private static Lifecycle _lifecycle;

        // simple indexing strategy just asks the operation for its result
        private readonly SimpleIndexingStrategy _simpleIndexingStrategy = new SimpleIndexingStrategy();

        private readonly IDocumentCreationStrategy _documentCreationStrategy;

        public AssetIndexer(IIndexDirectoryFactory indexDirectoryFactory, IAssetDocumentFactory assetDocumentFactory)
        {
            _lifecycle = new Lifecycle(indexDirectoryFactory);
            _documentCreationStrategy = new DefaultDocumentCreationStrategy(assetDocumentFactory);
            AssetDocumentFactory = assetDocumentFactory;
        }

        public IIndexResult IndexAssets(IEnumerable<IAsset> assets)
        {
            return IndexAssets(assets, AssetIndexingParams.Index_All);
        }

        public IIndexResult IndexAssets(IEnumerable<IAsset> assets, AssetIndexingParams assetIndexingParams)
        {
            return Perform(assets, _simpleIndexingStrategy, new IndexAssetsOperation(this, UpdateMode.Interactive, assetIndexingParams));
        }

        public IIndexResult DeIndexAssets(IEnumerable<IAsset> assets)
        {
            // As per http://stackoverflow.com/a/3894582. The IndexWriter is CPU bound, so we can try and write multiple packages in parallel.
            // The IndexWriter is thread safe and is primarily CPU-bound.
            Parallel.ForEach(assets, DeIndexAction);

            throw new NotImplementedException();
        }

        public IIndexResult ReIndexAssets(IEnumerable<Api.Models.Asset> assets, bool reIndexComments, bool conditionalUpdate)
        {
            var assetIndexingParams = AssetIndexingParams.Builder().SetComments(reIndexComments).Build();
            return ReIndexAssets(assets, assetIndexingParams, conditionalUpdate);
        }

        public IIndexResult ReIndexAssets(IEnumerable<Api.Models.Asset> assets, AssetIndexingParams assetIndexingParams, bool conditionalUpdate)
        {
            var operation = new ReIndexAssetsOperation(assetIndexingParams, conditionalUpdate, _documentCreationStrategy);
            return Perform(assets, _simpleIndexingStrategy, operation);
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

        /// <summary>
        /// Used when indexing to do the actual indexing of an asset.
        /// </summary>
        private class IndexAssetsOperation : IIndexOperation
        {
            private readonly AssetIndexer _assetIndexer;
            private readonly UpdateMode _mode;
            private readonly AssetIndexingParams _assetIndexingParams;

            public IndexAssetsOperation(AssetIndexer outerInstance, UpdateMode mode, AssetIndexingParams assetIndexingParams)
            {
                _assetIndexer = outerInstance;
                _mode = mode;
                _assetIndexingParams = assetIndexingParams;
            }

            public IIndexResult Perform(IAsset asset)
            {
                try
                {
                    Documents documents = _assetIndexer._documentCreationStrategy.Get(asset, _assetIndexingParams);
                    Operation assetCreate = Operations.NewCreate(documents.Asset, _mode);
                    Operation onCompletion = Operations.NewCompletionDelegate(assetCreate, null);
                    var results = new AccumulatingResultBuilder();
                    results.Add("Asset", asset.Id, _lifecycle.AssetIndex.Perform(onCompletion));
                    if (documents.Comments.Any())
                    {
                        Operation commentsCreate = Operations.NewCreate(documents.Comments, _mode);
                        results.Add("Comment For Issue", asset.Id, _lifecycle.CommentIndex.Perform(commentsCreate));
                    }
                    //FlushCustomFieldValueCache();
                    return results.ToResult();
                }
                catch (Exception ex)
                {
                    return new Lucene.Index.Index.Failure(ex);
                }
            }
        }

        private class ReIndexAssetsOperation : IIndexOperation
        {
            private readonly AssetIndexingParams _assetIndexingParams;
            private readonly bool _conditionalUpdate;
            private readonly IDocumentCreationStrategy _documentCreationStrategy;

            public ReIndexAssetsOperation(AssetIndexingParams assetIndexingParams, bool conditionalUpdate, IDocumentCreationStrategy documentCreationStrategy)
            {
                _assetIndexingParams = assetIndexingParams;
                _conditionalUpdate = conditionalUpdate;
                _documentCreationStrategy = documentCreationStrategy;
            }

            public IIndexResult Perform(IAsset asset)
            {
                try
                {
                    using (Timeline.Capture("Index Asset: " + asset.Id))
                    {
                        var mode = UpdateMode.Interactive;
                        var documents = _documentCreationStrategy.Get(asset, _assetIndexingParams);
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

                        if (_assetIndexingParams.IndexComments)
                        {
                            results.Add("Comment for Asset", asset.Id,
                                _lifecycle.CommentIndex.Perform(Operations.NewUpdate(assetTerm, documents.Comments, mode)));
                        }

                        return results.ToResult();
                    }
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
            foreach (var manager in _lifecycle)
            {
                manager.DeleteIndexDirectory();
            }
        }

        public void DeleteIndexes(AssetIndexingParams assetIndexingParams)
        {
            var indexes = TransformIndexingParamsToIndexesEnumSet(assetIndexingParams);
            foreach (var indexName in indexes)
            {
                _lifecycle.Get(indexName).DeleteIndexDirectory();
            }
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

        public IList<string> IndexPaths => new List<string>();

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
        /// <returns>The <see cref="IIndexResult"/> may waited on or not.</returns>
        private static IIndexResult Perform(IEnumerable<IAsset> assets, IIndexingStrategy strategy, IIndexOperation operation)
        {
            using (strategy)
            {
                if (assets == null)
                {
                    throw new ArgumentNullException(nameof(assets));
                }
                 
                // thread-safe handler for the asynchronous Result
                var builder = new AccumulatingResultBuilder();

                Parallel.ForEach(assets, asset =>
                {
                    var supplier = new PerformSupplier(operation, asset);
                    var result = strategy.Get(supplier);
                    builder.Add("Asset", asset.Id, result);
                });

                // perform the operation for every asset in the collection
//                foreach (var asset in assets)
//                {
//                    var supplier = new PerformSupplier(operation, asset);
//                    var result = strategy.Get(supplier);
//                    builder.Add("Asset", asset.Id, result);
//                }

                return builder.ToResult();
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
                if (factory == null) throw new ArgumentNullException(nameof(factory));
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

            internal IIndex AssetIndex => Get(IndexDirectoryFactoryName.Asset).Index;

            internal IIndex CommentIndex => Get(IndexDirectoryFactoryName.Comment).Index;

            public IIndexManager Get(IndexDirectoryFactoryName key)
            {
                return Open()[key];
            }

            internal IEnumerable<string> IndexPaths => _factory.IndexPaths;

            internal string IndexRootPath => _factory.IndexRootPath;

            internal IndexDirectoryFactoryMode Mode
            {
                set { _factory.IndexingMode = value; }
            }
        }

        /// <summary>
        /// Get the list of change documents for indexing.
        /// </summary>
        private interface IDocumentBuilder : IEnumerable<Document>
        {
        }

        /// <summary>
        /// Get the documents (asset and asset related entities) for the asset.
        /// </summary>
        private class DefaultDocumentCreationStrategy : IDocumentCreationStrategy
        {
            private readonly IAssetDocumentFactory _assetDocumentFactory;

            public DefaultDocumentCreationStrategy(IAssetDocumentFactory assetDocumentFactory)
            {
                _assetDocumentFactory = assetDocumentFactory;
            }

            public Documents Get(IAsset asset, AssetIndexingParams assetIndexingParams)
            {
                var comments =  new List<Document>();
                var assets = _assetDocumentFactory.Apply(asset);
                return new Documents(asset, assets, comments);
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

        private IList<IndexDirectoryFactoryName> TransformIndexingParamsToIndexesEnumSet(AssetIndexingParams assetIndexingParams)
        {
            var indexes = new List<IndexDirectoryFactoryName>();

            if (assetIndexingParams.IndexAssets)
            {
                indexes.Add(IndexDirectoryFactoryName.Asset);
            }

            if (assetIndexingParams.IndexComments)
            {
                indexes.Add(IndexDirectoryFactoryName.Comment);
            }

            return indexes;
        }
    }
}