using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using QuoteFlow.Api.Lucene.Index;
using QuoteFlow.Api.Util;
using QuoteFlow.Core.Diagnostics;

namespace QuoteFlow.Core.Lucene.Index
{
    /// <summary>
    /// <see cref="IWriter"/> implementation that actually writes to an <see cref="IndexWriter"/>.
    /// </summary>
    public class WriterWrapper : IWriter
    {
        private readonly IndexWriter _writer;
        private readonly ISupplier<IndexSearcher> _indexSearchSupplier;

        // for testing, can't make this accept an IndexWriter without making main constructor throw IOException
        public WriterWrapper(ISupplier<IndexWriter> writerFactory, ISupplier<IndexSearcher> indexSearcherSupplier, 
            IndexEngine.FlushPolicy flushPolicy, long commitFrequency)
        {
            _writer = writerFactory.Get();
            _indexSearchSupplier = indexSearcherSupplier;
            FlushPolicy = flushPolicy;
            CommitFrequency = commitFrequency;
        }

        public WriterWrapper(IIndexConfiguration configuration, UpdateMode mode,
            ISupplier<IndexSearcher> indexSearcherSupplier) 
            : this(new ConfiguredIndexWriter(configuration, mode), indexSearcherSupplier, 
                  configuration.GetWriterSettings(mode).FlushPolicy, 
                  configuration.GetWriterSettings(mode).CommitFrequency)
        {
        }

        private class ConfiguredIndexWriter : ISupplier<IndexWriter>
        {
			private readonly IIndexConfiguration _configuration;
			private readonly UpdateMode _mode;

			public ConfiguredIndexWriter(IIndexConfiguration configuration, UpdateMode mode)
			{
				_configuration = configuration;
				_mode = mode;
			}

            public IndexWriter Get()
            {
                try
                {
                    var writerSettings = _configuration.GetWriterSettings(_mode);
                    // todo: lucene 4.8
                    //var luceneConfig = writerSettings.GetWriterConfiguration(_configuration.Analyzer);
                    //return new IndexWriter(_configuration.Directory, luceneConfig);

                    var writer = new IndexWriter(_configuration.Directory, _configuration.Analyzer, true,
                        new IndexWriter.MaxFieldLength(writerSettings.MaxFieldLength));
                    writer.MaxMergeDocs = writerSettings.MaxMergeDocs;
                    writer.MergeFactor = writerSettings.MergeFactor;
                    writer.SetMaxBufferedDocs(writerSettings.MaxBufferedDocs);
                    writer.SetInfoStream(new DebugTextWriter());
                    writer.SetMergePolicy(writerSettings.GetMergePolicy(writer));

//                    return new IndexWriter(_configuration.Directory, _configuration.Analyzer, true,
//                        new IndexWriter.MaxFieldLength(writerSettings.MaxFieldLength));

                    return writer;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public IWriter Get(UpdateMode mode)
        {
            return this;
        }

        public IndexWriter GetLuceneWriter()
        {
            return _writer;
        }

        public void AddDocuments(IEnumerable<Document> documents)
        {
//            foreach (Document document in documents)
//            {
//                _writer.AddDocument(document);
//            }

            // todo: lucene perf?

            // As per http://stackoverflow.com/a/3894582. The IndexWriter is CPU bound, so we can try and write multiple packages in parallel.
            // The IndexWriter is thread safe and is primarily CPU-bound.
            Parallel.ForEach(documents, doc => _writer.AddDocument(doc));
        }

        public void DeleteDocuments(Term identifyingTerm)
        {
            _writer.DeleteDocuments(identifyingTerm);
        }

        public void UpdateDocuments(Term identifyingTerm, IEnumerable<Document> documents)
        {
            documents = documents.ToList();

            if (documents.Count() == 1)
            {
                _writer.UpdateDocument(identifyingTerm, documents.First());
            }
            else
            {
                _writer.DeleteDocuments(identifyingTerm);
                AddDocuments(documents);
            }
        }

        public void UpdateDocumentConditionally(Term identifyingTerm, Document document, string optimisticLockField)
        {
            // use the specified field as an optimistic locking check
            var updateCondition = new BooleanQuery(true)
            {
                new BooleanClause(new TermQuery(identifyingTerm), Occur.MUST),
                new BooleanClause(new TermRangeQuery(optimisticLockField, null, document.Get(optimisticLockField), true, true), Occur.MUST)
            };

            // try to reuse searchers
            var searcher = _indexSearchSupplier.Get();
            try
            {
                // if we have a matching document, that means that the document we are about to write is at least as
                // up-to-date as what is already in there (this check only works because there is a single thread updating the
                // index). so if we have a hit then go ahead and update, otherwise this update is a NOP.
                TopDocs topDocs = searcher.Search(updateCondition, 1);
                if (topDocs.TotalHits > 0)
                {
                    _writer.UpdateDocument(identifyingTerm, document);
                }
            }
            finally
            {
                CloseQuietly(searcher);
            }
        }

        public void Optimize()
        {
            _writer.Optimize();
        }

        public virtual void Commit()
        {
            try
            {
                _writer.Commit();
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }

        public IndexEngine.FlushPolicy FlushPolicy { get; set; }

        public long CommitFrequency { get; private set; }

        [Obsolete("Use Dispose() instead.")]
        public void Close()
        {
            try
            {
                _writer.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void Dispose()
        {
            try
            {
                _writer.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void CloseQuietly(IndexSearcher searcher)
        {
            try
            {
                searcher?.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error closing: " + searcher, ex);
                //QuietLog.LogError(ex);
            }
        }
    }
}
