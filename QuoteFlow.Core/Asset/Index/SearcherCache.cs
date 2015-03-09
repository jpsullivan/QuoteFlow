using System.IO;
using System.Threading;
using Lucene.Net.Index;
using Lucene.Net.Search;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Core.Asset.Index
{
    /// <summary>
    /// This class manages the searcher thread local cache. The actual searchers themselves are
    /// stored in this object, which is stored in a <seealso cref="ThreadLocal{T}"/>.
    /// </summary>
    internal class SearcherCache
    {
        private static readonly ThreadLocal<SearcherCache> ThreadLocal = new ThreadLocal<SearcherCache>();

        private SearcherCache()
        {
        }

        internal static SearcherCache ThreadLocalCache
        {
            get
            {
                SearcherCache threadLocalSearcherCache = ThreadLocal.get();
                if (threadLocalSearcherCache == null)
                {
                    threadLocalSearcherCache = new SearcherCache();
                    ThreadLocal.set(threadLocalSearcherCache);
                }
                return threadLocalSearcherCache;
            }
        }

        private IndexSearcher assetSearcher;
        private IndexSearcher commentSearcher;

        internal virtual IndexSearcher RetrieveAssetSearcher(ISupplier<IndexSearcher> searcherSupplier)
        {
            if (assetSearcher == null)
            {
                assetSearcher = searcherSupplier.Get();
            }

            return assetSearcher;
        }

        internal virtual IndexReader RetrieveAssetReader(ISupplier<IndexSearcher> searcherSupplier)
        {
            return RetrieveAssetSearcher(searcherSupplier).IndexReader;
        }

        internal virtual IndexSearcher RetrieveCommentSearcher(ISupplier<IndexSearcher> searcherSupplier)
        {
            if (commentSearcher == null)
            {
                commentSearcher = searcherSupplier.Get();
            }

            return commentSearcher;
        }

        /// <summary>
        /// Close the issues and comments searchers.
        /// </summary>
        /// <exception cref="IOException"> if there's a lucene exception accessing the disk </exception>
        internal virtual void CloseSearchers()
        {
            try
            {
                CloseSearcher(assetSearcher);
            }
            finally
            {
                // if close throws an IOException, we still need to null the searcher
                assetSearcher = null;

                try
                {
                    CloseSearcher(commentSearcher);
                }
                finally
                {
                    // if close throws an IOException, we still need to null the searcher (JRA-10423)
                    commentSearcher = null;
                }

            }
        }

        private void CloseSearcher(IndexSearcher searcher)
        {
            if (searcher != null)
            {
                searcher.Dispose();
            }
        }
    }

}