using System;
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
using QuoteFlow.Core.Configuration.Lucene;
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
        public void TestSearcherClosed()
        {
            var configuration = new IndexConfiguration(new RAMDirectory(), new StandardAnalyzer(LuceneVersion.Get()));

            _indexSearcherSupplier = new SimpleEngine();
            var searcherFactory = ToFactory(new SimpleEngine());

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

        private class SimpleEngine : ISupplier<IndexSearcher>
        {
            public IndexSearcher Get()
            {
                return MockSearcherFactory.GetCleanSearcher();
            }
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

        private void AssertReaderClosed(IndexReader reader)
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

        private void AssertReaderOpen(IndexReader reader)
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

        private IndexEngine GetRamDirectory()
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