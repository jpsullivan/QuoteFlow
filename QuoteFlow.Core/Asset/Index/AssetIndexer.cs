using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lucene.Net.Search;
using QuoteFlow.Api.Infrastructure.Concurrency;
using QuoteFlow.Api.Lucene.Index;
using QuoteFlow.Api.Models;
using QuoteFlow.Core.Index;
using QuoteFlow.Core.Lucene.Index;

namespace QuoteFlow.Core.Asset.Index
{
    public class AssetIndexer : IAssetIndexer
    {
        public IAssetDocumentFactory AssetDocumentFactory { get; protected set; }

        private readonly Lifecycle _lifecycle;

        // simple indexing strategy just asks the operation for its result
        private readonly SimpleIndexingStrategy _simpleIndexingStrategy = new SimpleIndexingStrategy();

        public AssetIndexer(IIndexDirectoryFactory indexDirectoryFactory, IAssetDocumentFactory assetDocumentFactory)
        {
            _lifecycle = new Lifecycle(indexDirectoryFactory);
            AssetDocumentFactory = assetDocumentFactory;
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
            var mode = UpdateMode.Interactive;
            var documents = DocumentCreationStrategy
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
        /// Perform an <seealso cref="IndexOperation"/> on some <seealso cref="EnclosedIterable issues"/> using a particular {@link
        /// IndexingStrategy strategy}. There is a <seealso cref="Context task context"/> that must be updated to provide feedback to
        /// the user.
        /// <p/>
        /// The implementation needs to be thread-safe, as it may be run in parallel and maintain a composite result to
        /// return to the caller.
        /// </summary>
        /// <param name="assets"> the issues to index/deindex/reindex </param>
        /// <param name="operation"> deindex/reindex/index etc. </param>
        /// <returns> the <seealso cref="IIndexResult"/> may waited on or not. </returns>
        private static IIndexResult Perform(IEnumerable<IAsset> assets, Operation operation)
        {
            try
            {
                if (assets == null) throw new ArgumentNullException("assets");

                // thread-safe handler for the asynchronous Result
                AccumulatingResultBuilder builder = new AccumulatingResultBuilder();
                
                // perform the operation for every asset in the collection
                foreach (var asset in assets)
                {
                    
                }
                
                return builder.ToResult();
            }
            finally
            {
                //strategy.close();
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
    }
}