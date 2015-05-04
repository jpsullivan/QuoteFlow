using Lucene.Net.Analysis;
using Lucene.Net.Store;
using QuoteFlow.Core.Asset.Index;
using QuoteFlow.Core.Lucene.Index;

namespace QuoteFlow.Core.Index
{
    /// <summary>
    /// Static factory class for creating <see cref="IIndex"/> and 
    /// <see cref="IIndexManager"/> instances.
    /// </summary>
    public class Indexes
    {
        /// <summary>
        /// Creates an index where the index operations are placed on a queue and the actual work 
        /// is done on a background thread. Any <see cref="AssetDocumentAndIdCollector.Result"/> may be
        /// waited on to make sure that subsequent searchers will see the result of that update, but 
        /// you can timeout on that without losing the update.
        /// </summary>
        /// <param name="name">Used to name the background thread.</param>
        /// <param name="config">Holds the <see cref="Directory"/> and <see cref="Analyzer"/> used for indexing and searching.</param>
        /// <param name="maxQueueSize"></param>
        /// <returns>A <see cref="IIndexManager"/> that has an index configured for queued operations.</returns>
        public static IIndexManager CreateQueuedIndexManager(string name, IIndexConfiguration config, long maxQueueSize)
        {
            // writePolicy is that the IndexWriter is committed after every write
            var engine = new IndexEngine(config, IndexEngine.FlushPolicy.Flush);
            //return new DefaultIndexManager(config, engine, new QueueingIndex(name, new Lucene.Index.Index(engine), maxQueueSize));
            return new DefaultIndexManager(config, engine, new Lucene.Index.Index(engine));
        }

        /// <summary>
        /// Creates an index where the index operation work is done in the calling thread. 
        /// Any <see cref="AssetDocumentAndIdCollector.Result"/> may be waited on but it will always be a
        /// non-blocking operation as it will be complete already. There is no way to timeout these operations.
        /// 
        /// The Index write policy is that flushes will only occur if a Searcher is requested, when the 
        /// IndexWriter decides to according to its internal buffering policy, or when the index is closed.
        /// </summary>
        /// <param name="config">Holds the <see cref="Directory"/> and <see cref="Analyzer"/> used for indexing and searching.</param>
        /// <returns>A <see cref="IIndexManager"/> that has an index configured for direct operations.</returns>
        public static IIndexManager CreateSimpleIndexManager(IIndexConfiguration config)
        {
            var engine = new IndexEngine(config, IndexEngine.FlushPolicy.None);
            return new DefaultIndexManager(config, engine, new Lucene.Index.Index(engine));
        }
    }
}