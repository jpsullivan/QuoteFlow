using Lucene.Net.Index;
using Lucene.Net.Search;
using Moq;
using QuoteFlow.Api.Util;
using QuoteFlow.Core.Asset.Index;
using QuoteFlow.Core.Tests.Jql.Util.Searchers;
using Xunit;

namespace QuoteFlow.Core.Tests.Asset.Index
{
    public class SearcherCacheTests
    {
        // Check that the ThreadLocalSearcherCache is only cached once per request-cache
        [Fact]
        public void TestGetSearcher()
        {
            var supplier = new MockIndexSearcher();

            SearcherCache cache = SearcherCache.ThreadLocalCache;
            IndexSearcher searcher = cache.RetrieveAssetSearcher(supplier);
            IndexSearcher anotherSearcher = cache.RetrieveAssetSearcher(supplier);

            Assert.Same(searcher, anotherSearcher);

            cache.CloseSearchers();
            IndexSearcher newSearcher = cache.RetrieveAssetSearcher(supplier);

            Assert.NotSame(searcher, newSearcher);
        }

        // Check that the ThreadLocalSearcherCache is only cached once per request-cache
        [Fact]
        public void TestGetCommentSearcher()
        {
            var supplier = new MockIndexSearcher();

            SearcherCache cache = SearcherCache.ThreadLocalCache;
            IndexSearcher searcher = cache.RetrieveCommentSearcher(supplier);
            IndexSearcher anotherSearcher = cache.RetrieveCommentSearcher(supplier);

            Assert.Same(searcher, anotherSearcher);

            cache.CloseSearchers();
            IndexSearcher newSearcher = cache.RetrieveCommentSearcher(supplier);

            Assert.NotSame(searcher, newSearcher);
        }

        [Fact]
        public void TestGetReader()
        {
            var supplier = new MockIndexSearcher();

            SearcherCache cache = SearcherCache.ThreadLocalCache;
            IndexReader reader = cache.RetrieveAssetReader(supplier);
            IndexReader anotherReader = cache.RetrieveAssetReader(supplier);

            Assert.Same(reader, anotherReader);

            cache.CloseSearchers();
            IndexReader newReader = cache.RetrieveAssetReader(supplier);

            Assert.NotSame(reader, newReader);
        }

        [Fact]
        public void TestResetSearchers()
        {
            SearcherCache cache = SearcherCache.ThreadLocalCache;
            cache.CloseSearchers();

            var mockAssetSearcherControl = new Mock<IndexSearcher>(MockSearcherFactory.GetCleanRamDirectory());
            var mockCommentSearcherControl = new Mock<IndexSearcher>(MockSearcherFactory.GetCleanRamDirectory());
            IndexSearcher mockAssetSearcher = mockAssetSearcherControl.Object;
            IndexSearcher mockCommentSearcher = mockCommentSearcherControl.Object;

            mockAssetSearcher.Dispose();
            mockCommentSearcher.Dispose();

            cache.RetrieveAssetSearcher(() => mockAssetSearcher);
            cache.RetrieveCommentSearcher(() => mockCommentSearcher);
            cache.CloseSearchers();

            mockAssetSearcherControl.Verify();
            mockCommentSearcherControl.Verify();
        }

        [Fact]
        public void TestResetNullSearchers()
        {
            // nothing to assert really. Just need to make sure this test passes w/out any exceptions
            SearcherCache.ThreadLocalCache.CloseSearchers();
        }

        private class MockIndexSearcher : ISupplier<IndexSearcher>
        {
            public IndexSearcher Get()
            {
                return MockSearcherFactory.GetCleanSearcher();
            }
        }
    }
}