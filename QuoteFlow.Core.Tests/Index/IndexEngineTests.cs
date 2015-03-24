using System.Text;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Moq;
using QuoteFlow.Api.Lucene.Index;
using QuoteFlow.Api.Util;
using QuoteFlow.Api.Util.Support;
using QuoteFlow.Core.Index;
using QuoteFlow.Core.Lucene.Index;
using QuoteFlow.Core.Tests.Jql.Util.Searchers;
using Xunit;

namespace QuoteFlow.Core.Tests.Index
{
    public class IndexEngineTests
    {
        private static ISupplier<IndexSearcher> _indexSearcherSupplier;

        [Fact]
        public void TestSearcherDisposed()
        {
            var configuration = new IndexConfiguration(new RAMDirectory(), new StandardAnalyzer(LuceneVersion.Get()));

            var mockSupplier = new Mock<ISupplier<IndexSearcher>>();
            mockSupplier.Setup(x => x.Get()).Returns(MockSearcherFactory.GetCleanSearcher);

            _indexSearcherSupplier = mockSupplier.Object;
            var searcherFactory = ToFactory(mockSupplier.Object);

            var writer = new Mock<IWriter>();
            writer.Setup(x => x.Get(UpdateMode.Interactive))
                .Callback(() => new WriterWrapper(configuration, UpdateMode.Interactive, _indexSearcherSupplier));
            var engine = new IndexEngine(searcherFactory, writer.Object, configuration, IndexEngine.FlushPolicy.None);

            var searcher = engine.Searcher;

            // should be same until something is written
            Assert.Same(searcher, engine.Searcher);
            Touch(engine);

            var newSearcher = engine.Searcher;
            Assert.NotSame(searcher, newSearcher);
        }

        [Fact]
        public void TestWriterNotFlushedForWritePolicyNone()
        {
            var configuration = new IndexConfiguration(new RAMDirectory(), new StandardAnalyzer(LuceneVersion.Get()));
            var mockWriterWrapper = new Mock<IWriter>();
            mockWriterWrapper.Setup(x => x.Get(It.IsAny<UpdateMode>())).Returns(new DummyWriterWrapper(configuration, _indexSearcherSupplier));
            var engine = new IndexEngine(new DummySearcherFactory(), mockWriterWrapper.Object, configuration, IndexEngine.FlushPolicy.None);

            Touch(engine);
        }

        #region Dummy WriterWrapper helper

        private class DummyWriterWrapper : WriterWrapper
        {
            public DummyWriterWrapper(IIndexConfiguration configuration, ISupplier<IndexSearcher> indexSearcherSupplier)
                : base(configuration, UpdateMode.Interactive, indexSearcherSupplier)
            {
            }

            public override void Dispose()
            {
                Assert.True(false, "should not dispose!");
            }

            public override void Commit()
            {
                Assert.True(false, "should not commit!");
            }
        }

        #endregion

        [Fact]
        public void TestWriterClosedForWritePolicyClose()
        {
            var configuration = new IndexConfiguration(new RAMDirectory(), new StandardAnalyzer(LuceneVersion.Get()));
            var writerWrapper = new DisposeCountingWriterWrapper(configuration, _indexSearcherSupplier);
            var mockWriterWrapper = new Mock<IWriter>();
            mockWriterWrapper.Setup(x => x.Get(It.IsAny<UpdateMode>())).Returns(writerWrapper);
            var engine = new IndexEngine(new DummySearcherFactory(), mockWriterWrapper.Object, configuration, IndexEngine.FlushPolicy.Close);

            Touch(engine);

            Assert.Equal(1, writerWrapper.CloseCount);
        }

        public class TheDisposeMethod
        {
            [Fact]
            public void TestOldReaderDisposed_When_SearcherDisposedBeforeEngine()
            {
                var engine = GetRamDirectory();
                var searcher = engine.Searcher;
                var reader = searcher.IndexReader;
                AssertReaderOpen(reader);
                engine.Dispose();
                AssertReaderOpen(reader);

                searcher.Dispose();
                AssertReaderClosed(reader);
            }

            [Fact]
            public void TestOldReaderClosed_When_SearcherClosedAfterEngine()
            {
                var engine = GetRamDirectory();
                var searcher = engine.Searcher;
                var reader = searcher.IndexReader;
                AssertReaderOpen(reader);
                searcher.Dispose();
                AssertReaderOpen(reader);

                engine.Dispose();
                AssertReaderClosed(reader);
            }

            private static readonly AtomicInteger SearcherDisposeCount = new AtomicInteger();

            [Fact]
            public void TestWriterAndSearcherDisposedWhenDisposed()
            {
                var directory = new RAMDirectory();
                var configuration = new IndexConfiguration(directory, new StandardAnalyzer(LuceneVersion.Get()));
                var writerWrapper = new DisposeCountingWriterWrapper(configuration, _indexSearcherSupplier);

                // instead of create helper classes, lets just mock the neceessary functionality to count disposals
                var mockSupplier = new Mock<ISupplier<IndexSearcher>>();
                mockSupplier.Setup(x => x.Get()).Returns(new DisposalCounterIndexSearcher(directory));

                var mockWriter = new Mock<IWriter>();
                mockWriter.Setup(x => x.Get(It.IsAny<UpdateMode>())).Returns(writerWrapper);

                _indexSearcherSupplier = mockSupplier.Object;
                var searcherFactory = ToFactory(mockSupplier.Object);

                var engine = new IndexEngine(searcherFactory, mockWriter.Object, configuration, IndexEngine.FlushPolicy.Close);

                Touch(engine);

                Assert.Equal(1, writerWrapper.CloseCount);
                Assert.Equal(0, SearcherDisposeCount.Get());

                var searcher = engine.Searcher;
                searcher.Dispose(); // todo: this not disposing on the DelayCloseSearcher
                searcher.Dispose();
//                engine.Searcher.Dispose();
//                engine.Searcher.Dispose();
                Assert.Equal(2, writerWrapper.CloseCount);
                Assert.Equal(0, SearcherDisposeCount.Get());

                engine.Dispose();
                Assert.Equal(2, writerWrapper.CloseCount);
                Assert.Equal(1, SearcherDisposeCount.Get());
            }

            private class DisposalCounterIndexSearcher : IndexSearcher
            {
                public DisposalCounterIndexSearcher(Directory path)
                    : base(path)
                {
                }

                protected override void Dispose(bool disposing)
                {
                    SearcherDisposeCount.IncrementAndGet();
                    base.Dispose(disposing);
                }
            }
        }

        private class DisposeCountingWriterWrapper : WriterWrapper
        {
            private AtomicInteger _count = new AtomicInteger();

            public DisposeCountingWriterWrapper(IIndexConfiguration configuration, ISupplier<IndexSearcher> indexSearcherSupplier) 
                : base(configuration, UpdateMode.Interactive, indexSearcherSupplier)
            {
            }

            public override void Dispose()
            {
                _count.IncrementAndGet();
                base.Dispose();
            }

            public int CloseCount
            {
                get { return _count.Get(); }
            }
        }

        private class DummySearcherFactory : IndexEngine.ISearcherFactory
        {
            public IndexSearcher Get()
            {
                Assert.True(false, "no searcher required");
                return null;
            }

            public void Release()
            {
            }
        }

        #region Lucene reader / document helpers

        private static void AssertReaderClosed(IndexReader reader)
        {
            // if the reader is closed, flush will throw an AlreadyClosedException
            try
            {
                reader.Flush();
                Assert.True(false, "The reader should have been closed after a write when we get a new searcher");
            }
            catch (AlreadyClosedException e)
            {
                Assert.True(true);
            }
        }

        private static void AssertReaderOpen(IndexReader reader)
        {
            // if the reader is closed, flush will throw an AlreadyClosedException
            try
            {
                reader.Flush();
            }
            catch (AlreadyClosedException e)
            {
                Assert.True(false, "The reader should not have been closed.");
            }
        }

        private void WriteTestDocument(IndexEngine engine)
        {
            var doc = new Document();
            var bytes = Encoding.UTF8.GetBytes("bytes");
            doc.Add(new Field("test", bytes, Field.Store.YES));
            engine.Write(Operations.NewCreate(doc, UpdateMode.Interactive));
        }

        private static IndexEngine GetRamDirectory()
        {
            var directory = new RAMDirectory();
            // todo: lucene 4.8
//            IndexWriterConfig conf = new IndexWriterConfig(LuceneVersion.get(), new StandardAnalyzer(LuceneVersion.get()));
//            conf.OpenMode = IndexWriterConfig.OpenMode.CREATE;
//            (new IndexWriter(directory, conf)).close();
            (new IndexWriter(directory, new StandardAnalyzer(LuceneVersion.Get()), new IndexWriter.MaxFieldLength(1000))).Dispose();
            var configuration = new IndexConfiguration(directory, new StandardAnalyzer(LuceneVersion.Get()));
            return new IndexEngine(configuration, IndexEngine.FlushPolicy.Flush);
        }

        #endregion

        private static void Touch(IndexEngine engine)
        {
            engine.Write(new TouchOperation());
        }

        private class TouchOperation : Operation
        {
            public override void Perform(IWriter writer)
            {
            }

            public override UpdateMode Mode()
            {
                return UpdateMode.Interactive;
            }
        }

        #region ToFactory Helper

        private static IndexEngine.ISearcherFactory ToFactory(ISupplier<IndexSearcher> indexSearcherSupplier)
        {
            if (indexSearcherSupplier is IndexEngine.ISearcherFactory)
            {
                return (IndexEngine.ISearcherFactory) indexSearcherSupplier;
            }

            return new SimpleSearcherFactory();
        }

        private class SimpleSearcherFactory : IndexEngine.ISearcherFactory
        {
            public IndexSearcher Get()
            {
                return _indexSearcherSupplier.Get();
            }

            public void Release()
            {
            }
        }

        #endregion
    }
}