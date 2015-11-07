using Lucene.Net.Search;
using Lucene.Net.Store;
using QuoteFlow.Core.Lucene.Index;
using QuoteFlow.Core.Tests.Jql.Util.Searchers;
using Xunit;

namespace QuoteFlow.Core.Tests.Index
{
    public class DelegateSearcherTests
    {
        [Fact]
        public void TestGet()
        {
            var searcher = new IndexSearcher(GetDirectory());
            Assert.Same(searcher, new DelegateSearcher(searcher).Get());
        }

        [Fact]
        public void TestGetHashCode()
        {
            var searcher = new IndexSearcher(GetDirectory());
            Assert.Equal(searcher.GetHashCode(), new DelegateSearcher(searcher).GetHashCode());
        }

        [Fact]
        public void TestToString()
        {
            var searcher = new IndexSearcher(GetDirectory());
            Assert.Equal(searcher.ToString(), new DelegateSearcher(searcher).ToString());
        }

        private Directory GetDirectory()
        {
            return MockSearcherFactory.GetCleanRamDirectory();
        }
    }
}
