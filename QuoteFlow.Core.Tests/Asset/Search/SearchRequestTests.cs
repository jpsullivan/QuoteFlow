using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Models;
using Xunit;

namespace QuoteFlow.Core.Tests.Asset.Search
{
    public class SearchRequestTests
    {
        [Fact]
        public void TestBlankCtor()
        {
            var sr = new SearchRequest();
            Assert.NotNull(sr.Query);
        }

        public class TheModifiedProperty
        {
            [Fact]
            public void FalseFromNewCtor()
            {
                var sr = new SearchRequest();
                Assert.False(sr.Modified);
            }

            [Fact]
            public void FalseFromNewCtorWithQuery()
            {
                var sr = new SearchRequest(new Query());
                Assert.False(sr.Modified);
            }

            [Fact]
            public void TrueAfterSettingName()
            {
                var sr = new SearchRequest {Name = "test"};
                Assert.True(sr.Modified);
            }

            [Fact]
            public void TrueAfterSettingDescription()
            {
                var sr = new SearchRequest { Description = "test" };
                Assert.True(sr.Modified);
            }

            [Fact]
            public void TrueAfterSettingOwner()
            {
                var sr = new SearchRequest { Owner = new User() };
                Assert.True(sr.Modified);
            }

            [Fact]
            public void TrueAfterSettingQuery()
            {
                var sr = new SearchRequest { Query = new Query() };
                Assert.True(sr.Modified);
            }
        }
    }
}