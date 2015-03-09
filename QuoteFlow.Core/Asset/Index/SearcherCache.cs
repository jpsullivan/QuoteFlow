using System;
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
    public class SearcherCache
    {
        private static ThreadLocal<SearcherCache> _threadLocal = new ThreadLocal<SearcherCache>();

        private SearcherCache()
        {
        }

        public static SearcherCache ThreadLocalCache
        {
            get
            {
                SearcherCache threadLocalSearcherCache = _threadLocal.Value;
                if (threadLocalSearcherCache != null)
                {
                    return threadLocalSearcherCache;
                }

                threadLocalSearcherCache = new SearcherCache();
                _threadLocal = new ThreadLocal<SearcherCache>(() => threadLocalSearcherCache);
                return threadLocalSearcherCache;
            }
        }

        private IndexSearcher _assetSearcher;
        private IndexSearcher _commentSearcher;

        public IndexSearcher RetrieveAssetSearcher(ISupplier<IndexSearcher> searcherSupplier)
        {
            return _assetSearcher ?? (_assetSearcher = searcherSupplier.Get());
        }

        public IndexSearcher RetrieveAssetSearcher(Func<IndexSearcher> searcherSupplier)
        {
            return _assetSearcher ?? (_assetSearcher = searcherSupplier());
        }

        public IndexReader RetrieveAssetReader(ISupplier<IndexSearcher> searcherSupplier)
        {
            return RetrieveAssetSearcher(searcherSupplier).IndexReader;
        }

        public IndexSearcher RetrieveCommentSearcher(ISupplier<IndexSearcher> searcherSupplier)
        {
            return _commentSearcher ?? (_commentSearcher = searcherSupplier.Get());
        }

        public IndexSearcher RetrieveCommentSearcher(Func<IndexSearcher> searcherSupplier)
        {
            return _commentSearcher ?? (_commentSearcher = searcherSupplier());
        }

        /// <summary>
        /// Close the issues and comments searchers.
        /// </summary>
        /// <exception cref="IOException"> if there's a lucene exception accessing the disk </exception>
        public void CloseSearchers()
        {
            try
            {
                CloseSearcher(_assetSearcher);
            }
            finally
            {
                // if close throws an IOException, we still need to null the searcher
                _assetSearcher = null;

                try
                {
                    CloseSearcher(_commentSearcher);
                }
                finally
                {
                    // if close throws an IOException, we still need to null the searcher (JRA-10423)
                    _commentSearcher = null;
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