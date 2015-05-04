using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Core.Asset.Search;
using Xunit;

namespace QuoteFlow.Core.Tests.Asset.Search
{
    public class SearchContextTests
    {
        private readonly ISearchContext _searchContext1;
        private readonly ISearchContext _searchContext2;

        public SearchContextTests()
        {
            _searchContext1 = new SearchContext();
            _searchContext2 = new SearchContext();
        }

        [Fact]
        public void TestEquals()
        {
            Assert.True(_searchContext1.Equals(_searchContext2));
            Assert.True(_searchContext2.Equals(_searchContext1));
            Assert.True(_searchContext1.Equals(_searchContext1));
            Assert.True(_searchContext2.Equals(_searchContext2));
        }
    }
}