using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using QuoteFlow.Api.Lucene.Index;
using QuoteFlow.Api.Util;
using QuoteFlow.Api.Util.Support;
using QuoteFlow.Core.Configuration.Lucene;
using QuoteFlow.Core.Lucene.Index;
using QuoteFlow.Core.Tests.Jql.Util.Searchers;
using Xunit;

namespace QuoteFlow.Core.Tests.Lucene.Index
{
    public class WriterWrapperTests
    {
        public class TheUpdateMethod
        {
            [Fact]
            public void UpdatesCorrectly()
            {
                var add = new AtomicInteger();
                var delete = new AtomicInteger();
                var update = new AtomicInteger();
                var configuration = new Configuration();
                var writer = new UpdateWriter(configuration.Directory, configuration.Analyzer,
                    new IndexWriter.MaxFieldLength(1000), add, delete, update);
                var wrapper = CreateWriterWrapper(writer);

                wrapper.UpdateDocuments(new Term("blah", "blah"), new List<Document> { new Document(), new Document(), new Document()});
                Assert.Equal(1, delete.Get());
                Assert.Equal(3, add.Get());
                Assert.Equal(0, update.Get());
                wrapper.UpdateDocuments(new Term("blah", "blah"), new List<Document> { new Document(), new Document() });
                Assert.Equal(2, delete.Get());
                Assert.Equal(5, add.Get());
                Assert.Equal(0, update.Get());
                wrapper.UpdateDocuments(new Term("blah", "blah"), new List<Document> { new Document() });
                Assert.Equal(2, delete.Get());
                Assert.Equal(5, add.Get());
                Assert.Equal(1, update.Get());
            }

            private class UpdateWriter : IndexWriter
            {
                private readonly AtomicInteger _add;
                private readonly AtomicInteger _delete;
                private readonly AtomicInteger _update;

                public UpdateWriter(Directory d, Analyzer a, MaxFieldLength mfl, AtomicInteger add, AtomicInteger delete, AtomicInteger update)
                    : base(d, a, mfl)
                {
                    _add = add;
                    _delete = delete;
                    _update = update;
                }

                public override void DeleteDocuments(Term term)
                {
                    _delete.IncrementAndGet();
                    base.DeleteDocuments(term);
                }

                public override void AddDocument(Document doc)
                {
                    _add.IncrementAndGet();
                    base.AddDocument(doc);
                }

                public override void UpdateDocument(Term term, Document doc)
                {
                    _update.IncrementAndGet();
                    base.UpdateDocument(term, doc);
                }
            }
        }

        private class Configuration : IIndexConfiguration
        {
            private readonly Directory _directory = MockSearcherFactory.GetCleanRamDirectory();

            public Directory Directory
            {
                get { return _directory; }
                set { }
            }

            public Analyzer Analyzer
            {
                get { return new StandardAnalyzer(LuceneVersion.Get()); }
                set { }
            }

            public WriterSettings GetWriterSettings(UpdateMode mode)
            {
                return IndexConfiguration.Default.Interactive;
            }
        }

        private static WriterWrapper CreateWriterWrapper(IndexWriter writer)
        {
            return new WriterWrapper(new WriterWrapperSupplier(writer), null, IndexEngine.FlushPolicy.Flush, 1L);
        }

        private class WriterWrapperSupplier : ISupplier<IndexWriter>
        {
            private readonly IndexWriter _writer;

            public WriterWrapperSupplier(IndexWriter writer)
            {
                _writer = writer;
            }

            public IndexWriter Get()
            {
                return _writer;
            }
        }
    }
}