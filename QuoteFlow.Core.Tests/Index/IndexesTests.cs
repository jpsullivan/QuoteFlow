using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using QuoteFlow.Api.Lucene.Index;
using QuoteFlow.Core.Index;
using QuoteFlow.Core.Lucene.Index;
using QuoteFlow.Core.Tests.Jql.Util.Searchers;
using Xunit;

namespace QuoteFlow.Core.Tests.Index
{
    public class IndexesTests
    {
        [Fact]
        public void TestQueuedManagerNewSearcherAfterCreate()
        {
            var manager = Indexes.CreateQueuedIndexManager("TestQueuedManager",
                new IndexConfiguration(MockSearcherFactory.GetCleanRamDirectory(),
                    new StandardAnalyzer(LuceneVersion.Get())), 100);
            try
            {
                var searcher = manager.OpenSearcher();
                manager.Index.Perform(Operations.NewCreate(new Document(), UpdateMode.Interactive));
                var searcher2 = manager.OpenSearcher();
                Assert.NotSame(searcher, searcher2);

                searcher.Dispose();
                searcher2.Dispose();
            }
            finally
            {
                manager.Dispose();
            }
        }

        [Fact]
        public void TestQueuedManagerNewSearcherAfterUpdate()
        {
            var manager = Indexes.CreateQueuedIndexManager("TestQueuedManager",
                new IndexConfiguration(MockSearcherFactory.GetCleanRamDirectory(),
                    new StandardAnalyzer(LuceneVersion.Get())), 100);
            try
            {
                var searcher = manager.OpenSearcher();
                manager.Index.Perform(Operations.NewUpdate(new Term("test", "1"), new Document(), UpdateMode.Interactive));
                var searcher2 = manager.OpenSearcher();
                Assert.NotSame(searcher, searcher2);

                searcher.Dispose();
                searcher2.Dispose();
            }
            finally
            {
                manager.Dispose();
            }
        }

        [Fact]
        public void TestSimpleManagerNewSearcherAfterDelete()
        {
            var manager = Indexes.CreateQueuedIndexManager("TestQueuedManager",
                new IndexConfiguration(MockSearcherFactory.GetCleanRamDirectory(),
                    new StandardAnalyzer(LuceneVersion.Get())), 100);
            try
            {
                var searcher = manager.OpenSearcher();
                manager.Index.Perform(Operations.NewDelete(new Term("test", "1"), UpdateMode.Interactive));
                var searcher2 = manager.OpenSearcher();
                Assert.NotSame(searcher, searcher2);

                searcher.Dispose();
                searcher2.Dispose();
            }
            finally
            {
                manager.Dispose();
            }
        }
    }
}