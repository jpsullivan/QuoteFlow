﻿using System.Text;
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
            var writerWrapper = new WriterWrapper(configuration, UpdateMode.Interactive, _indexSearcherSupplier);
            var engine = new IndexEngine(searcherFactory, writerWrapper, configuration);

            var writer = new Mock<IWriter>();
            writer.Setup(x => x.Get(UpdateMode.Interactive))
                .Callback(() => new WriterWrapper(configuration, UpdateMode.Interactive, _indexSearcherSupplier));
            

            var searcher = engine.GetSearcher();

            // should be same until something is written
            Assert.Same(searcher, engine.GetSearcher());
            Touch(engine);

            var newSearcher = engine.GetSearcher();
            Assert.NotSame(searcher, newSearcher);
        }

        [Fact]
        public void TestWriterNotFlushedForWritePolicyNone()
        {
            var configuration = new IndexConfiguration(new RAMDirectory(), new StandardAnalyzer(LuceneVersion.Get()));
            var writerWrapper = new DummyWriterWrapper(configuration, _indexSearcherSupplier);
            writerWrapper.FlushPolicy = IndexEngine.FlushPolicy.None;
            var mockWriterWrapper = new Mock<IWriter>();
            mockWriterWrapper.Setup(x => x.Get(It.IsAny<UpdateMode>())).Returns(writerWrapper);
            mockWriterWrapper.Setup(x => x.FlushPolicy).Returns(IndexEngine.FlushPolicy.None);
            var engine = new IndexEngine(new DummySearcherFactory(), mockWriterWrapper.Object, configuration);

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
            writerWrapper.FlushPolicy = IndexEngine.FlushPolicy.Close;
            var mockWriterWrapper = new Mock<IWriter>();
            mockWriterWrapper.Setup(x => x.Get(It.IsAny<UpdateMode>())).Returns(writerWrapper);
            mockWriterWrapper.Setup(x => x.FlushPolicy).Returns(IndexEngine.FlushPolicy.Close);
            var engine = new IndexEngine(new DummySearcherFactory(), mockWriterWrapper.Object, configuration);

            Touch(engine);

            Assert.Equal(1, writerWrapper.CloseCount);
        }

        public class TheDisposeMethod
        {
            [Fact]
            public void TestOldReaderDisposed_When_SearcherDisposedBeforeEngine()
            {
                var engine = GetRamDirectory();
                var searcher = engine.GetSearcher();
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
                var searcher = engine.GetSearcher();
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
                writerWrapper.FlushPolicy = IndexEngine.FlushPolicy.Close;

                // instead of create helper classes, lets just mock the neceessary functionality to count disposals
                var mockSupplier = new Mock<ISupplier<IndexSearcher>>();
                mockSupplier.Setup(x => x.Get()).Returns(new DisposalCounterIndexSearcher(directory));

                var mockWriter = new Mock<IWriter>();
                mockWriter.Setup(x => x.Get(It.IsAny<UpdateMode>())).Returns(writerWrapper);
                mockWriter.Setup(x => x.FlushPolicy).Returns(IndexEngine.FlushPolicy.Close);

                _indexSearcherSupplier = mockSupplier.Object;
                var searcherFactory = ToFactory(mockSupplier.Object);

                var engine = new IndexEngine(searcherFactory, mockWriter.Object, configuration);

                Touch(engine);

                Assert.Equal(1, writerWrapper.CloseCount);
                Assert.Equal(0, SearcherDisposeCount.Get());

                var searcher = engine.GetSearcher();
                searcher.Dispose();
                searcher.Dispose();
                Assert.Equal(1, writerWrapper.CloseCount);
                Assert.Equal(0, SearcherDisposeCount.Get());

                engine.Dispose();
                Assert.Equal(1, writerWrapper.CloseCount);
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

        public class TheCleanMethod
        {
            [Fact]
            public void TestDirectoryCleaned()
            {
                var directory = new RAMDirectory();
                var analyzer = new StandardAnalyzer(LuceneVersion.Get());

                {
                    // todo: lucene 4.8
                    //var conf = new IndexWriterConfig(LuceneVersion.Get(), analyzer);
                    IndexWriter writer = new IndexWriter(directory, analyzer, true, new IndexWriter.MaxFieldLength(1000));
                    writer.AddDocument(new Document());
                    writer.Dispose();
                }

                var configuration = new IndexConfiguration(directory, analyzer);
                var mockWriterWrapper = new Mock<IWriter>();
                mockWriterWrapper.Setup(x => x.Get(It.IsAny<UpdateMode>())).Callback(() => Assert.True(false, "no writer required"));
                mockWriterWrapper.Setup(x => x.FlushPolicy).Returns(IndexEngine.FlushPolicy.None);
                var engine = new IndexEngine(new DummySearcherFactory(), mockWriterWrapper.Object, configuration);

                Assert.Equal(1, new IndexSearcher(directory).IndexReader.NumDocs());
                engine.Clean();
                Assert.Equal(0, new IndexSearcher(directory).IndexReader.NumDocs());
            }
        }

        [Fact]
        public void TestSimpleFlowReaderIsClosed()
        {
            // test the simple flow of a get searcher / writer / dispose searcher / openSearcher
            // correctly gets a new reader and disposes the old reader.
            var engine = GetRamDirectory();

            var searcher = engine.GetSearcher();
            var reader = searcher.IndexReader;
            WriteTestDocument(engine);
            searcher.Dispose();

            var newSearcher = engine.GetSearcher();
            Assert.NotSame(searcher, newSearcher);
            var newReader = newSearcher.IndexReader;
            Assert.NotSame(reader, newReader);

            AssertReaderClosed(reader);
            AssertReaderOpen(newReader);
        }

        [Fact]
        public void TestMultipleSearchersWithoutWrites()
        {
            // test to just get the same searcher and reader when there are no writes
            var engine = GetRamDirectory();

            var searcher = engine.GetSearcher();
            var reader = searcher.IndexReader;
            searcher.Dispose();

            var newSearcher = engine.GetSearcher();
            Assert.Same(searcher, newSearcher);
            var newReader = newSearcher.IndexReader;
            Assert.Same(reader, newReader);
            AssertReaderOpen(reader);
        }

        [Fact]
        public void TestOldReaderStillOpenTillAllSearchersDisposed()
        {
            // test the old reader is still open until all searchers using it are disposed
            var engine = GetRamDirectory();

            var oldSearcher1 = engine.GetSearcher();
            var oldSearcher2 = engine.GetSearcher();
            var oldSearcher3 = engine.GetSearcher();
            var reader = oldSearcher1.IndexReader;
            Assert.Same(oldSearcher1, oldSearcher2); // should be same until something is written
            Assert.Same(oldSearcher1, oldSearcher3); // should be same until something is written

            WriteTestDocument(engine);

            oldSearcher1.Dispose();

            var newSearcher = engine.GetSearcher();
            Assert.NotSame(oldSearcher1, newSearcher);
            var newReader = newSearcher.IndexReader;
            Assert.NotSame(reader, newReader);

            AssertReaderOpen(reader);
            AssertReaderOpen(newReader);
            oldSearcher2.Dispose();
            AssertReaderOpen(reader);
            oldSearcher3.Dispose();
            AssertReaderClosed(reader);
            AssertReaderOpen(newReader);
        }

        [Fact]
        public void TestMultipleWritesBetweenSearcherDisposes()
        {
            // test the old reader is still open until all searchers using it are disposed
            // test the old reader is still open until all searchers using it are disposed
            var engine = GetRamDirectory();

            var oldSearcher1 = engine.GetSearcher();
            var oldSearcher2 = engine.GetSearcher();
            Assert.Same(oldSearcher1, oldSearcher2); // should be same until something is written.

            WriteTestDocument(engine);
            WriteTestDocument(engine);
            WriteTestDocument(engine);
            WriteTestDocument(engine);

            var oldSearcher3 = engine.GetSearcher();
            Assert.NotSame(oldSearcher1, oldSearcher3);

            WriteTestDocument(engine);
            WriteTestDocument(engine);

            var oldSearcher4 = engine.GetSearcher();
            var oldSearcher5 = engine.GetSearcher();
            Assert.NotSame(oldSearcher1, oldSearcher4);
            Assert.NotSame(oldSearcher3, oldSearcher4);
            Assert.Same(oldSearcher4, oldSearcher5);

            AssertReaderOpen(oldSearcher1.IndexReader);
            AssertReaderOpen(oldSearcher2.IndexReader);
            AssertReaderOpen(oldSearcher3.IndexReader);
            AssertReaderOpen(oldSearcher4.IndexReader);
            AssertReaderOpen(oldSearcher5.IndexReader);

            oldSearcher1.Dispose();
            oldSearcher2.Dispose();
            oldSearcher3.Dispose();
            oldSearcher4.Dispose();
            oldSearcher5.Dispose();

            AssertReaderClosed(oldSearcher1.IndexReader);
            AssertReaderClosed(oldSearcher2.IndexReader);
            AssertReaderClosed(oldSearcher3.IndexReader);
            AssertReaderOpen(oldSearcher4.IndexReader);
            AssertReaderOpen(oldSearcher5.IndexReader);

            WriteTestDocument(engine);
            AssertReaderOpen(oldSearcher4.IndexReader);

            // getting a new searcher -> gets a new reader -> disposes the old reader
            var oldSearcher6 = engine.GetSearcher();
            AssertReaderClosed(oldSearcher4.IndexReader);
            AssertReaderClosed(oldSearcher5.IndexReader);
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
            (new IndexWriter(directory, new StandardAnalyzer(LuceneVersion.Get()), true, new IndexWriter.MaxFieldLength(1000))).Dispose();
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