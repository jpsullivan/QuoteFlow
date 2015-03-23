using Lucene.Net.Analysis.Standard;
using Lucene.Net.Store;
using Moq;
using QuoteFlow.Api.Lucene.Index;
using QuoteFlow.Core.Index;
using QuoteFlow.Core.Lucene.Index;
using QuoteFlow.Core.Tests.Jql.Util.Searchers;
using Xunit;

namespace QuoteFlow.Core.Tests.Index
{
    public class DefaultIndexManagerTests
    {
        public class TheDeleteIndexDirectoryMethod
        {
            [Fact]
            public void CallsTheActorsCleanMethod()
            {
                var mockIndexEngine = new Mock<IndexEngine>();
                var mockIndex = new Mock<IDisposableIndex>();
                var configuration = new IndexConfiguration(MockSearcherFactory.GetCleanRamDirectory(), new StandardAnalyzer(LuceneVersion.Get()));
                var manager = new DefaultIndexManager(configuration, mockIndexEngine.Object, mockIndex.Object);

                manager.DeleteIndexDirectory();

                mockIndexEngine.Verify(x => x.Clean(), Times.Once);
            }
        }

        public class TheOpenSearcherMethod
        {
            [Fact]
            public void GetSearcherCallsEngine()
            {
                var mockIndexEngine = new Mock<IndexEngine>();
                var mockIndex = new Mock<IDisposableIndex>();
                var configuration = new IndexConfiguration(MockSearcherFactory.GetCleanRamDirectory(), new StandardAnalyzer(LuceneVersion.Get()));
                var manager = new DefaultIndexManager(configuration, mockIndexEngine.Object, mockIndex.Object);

                manager.OpenSearcher();

                mockIndexEngine.Verify(x => x.Searcher, Times.Once);
            }
        }

        public class TheDisposeMethod
        {
            [Fact]
            public void CloseCallsEngine()
            {
                var mockIndexEngine = new Mock<IndexEngine>();
                var mockIndex = new Mock<IDisposableIndex>();
                var configuration = new IndexConfiguration(MockSearcherFactory.GetCleanRamDirectory(), new StandardAnalyzer(LuceneVersion.Get()));
                var manager = new DefaultIndexManager(configuration, mockIndexEngine.Object, mockIndex.Object);

                manager.Dispose();

                mockIndex.Verify(x => x.Dispose(), Times.Once);
            }
        }

        [Fact]
        public void GetIndexReturnsSame()
        {
            var mockIndexEngine = new Mock<IndexEngine>();
            var mockIndex = new Mock<IDisposableIndex>();
            var configuration = new IndexConfiguration(MockSearcherFactory.GetCleanRamDirectory(), new StandardAnalyzer(LuceneVersion.Get()));
            var manager = new DefaultIndexManager(configuration, mockIndexEngine.Object, mockIndex.Object);

            Assert.Same(mockIndex.Object, manager.Index);
        }

        public class TheIndexCreatedProperty
        {
            [Fact]
            public void IndexCreatedShouldBeTrue()
            {
                var mockIndexEngine = new Mock<IndexEngine>();
                var mockIndex = new Mock<IDisposableIndex>();
                var configuration = new IndexConfiguration(MockSearcherFactory.GetCleanRamDirectory(), new StandardAnalyzer(LuceneVersion.Get()));
                var manager = new DefaultIndexManager(configuration, mockIndexEngine.Object, mockIndex.Object);

                Assert.True(manager.IndexCreated);
            }

            [Fact]
            public void IndexCreatedShouldBeFalse()
            {
                var mockIndexEngine = new Mock<IndexEngine>();
                var mockIndex = new Mock<IDisposableIndex>();
                var configuration = new IndexConfiguration(new RAMDirectory(), new StandardAnalyzer(LuceneVersion.Get()));
                var manager = new DefaultIndexManager(configuration, mockIndexEngine.Object, mockIndex.Object);

                Assert.False(manager.IndexCreated);
            }
        }
    }
}