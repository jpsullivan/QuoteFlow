using System;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Moq;
using QuoteFlow.Core.Lucene.Index;
using QuoteFlow.Core.Tests.Jql.Util.Searchers;
using Xunit;

namespace QuoteFlow.Core.Tests.Lucene.Index
{
    public class DelayCloseSearcherTests
    {
        public class TheCtor
        {
            [Fact]
            public void TestNullSearcher()
            {
                Assert.Throws<ArgumentNullException>(() => new DelayCloseSearcher(null));
            }
        }

        public class TheCloseWhenDoneMethod
        {
            [Fact(Skip = "todo implement once IndexSearcher Dispose is overridable")]
            public void TestCloseWhenDone()
            {
                var dir = MockSearcherFactory.GetCleanRamDirectory();
                var reader = IndexReader.Open(dir, true);
                var mockSearcher = new Mock<DelayCloseSearcher>(new IndexSearcher(reader));
                var searcher = mockSearcher.Object;
            }

            [Fact]
            public void TestCloseWhenDoneClosesImmediatelyIfNotOpen()
            {
                bool disposed = false;
                var dir = MockSearcherFactory.GetCleanRamDirectory();
                var reader = IndexReader.Open(dir, true);
                var searcher = new DelayCloseSearcher(new IndexSearcher(reader), () =>
                {
                    disposed = true;
                });

                searcher.CloseWhenDone();
                Assert.True(disposed);
            }

            [Fact]
            public void TestCloseWhenDoneDoesNotDisposeImmediatelyIfOpen()
            {
                bool disposed = false;
                var dir = MockSearcherFactory.GetCleanRamDirectory();
                var reader = IndexReader.Open(dir, true);
                var searcher = new DelayCloseSearcher(new IndexSearcher(reader), () =>
                {
                    disposed = true;
                });

                searcher.Open();
                searcher.CloseWhenDone();
                Assert.False(disposed);
                searcher.Dispose();
                Assert.True(disposed);
            }
        }
    }
}
