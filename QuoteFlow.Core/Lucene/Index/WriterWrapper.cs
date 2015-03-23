using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using QuoteFlow.Api.Lucene.Index;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Core.Lucene.Index
{
    /// <summary>
    /// <see cref="IWriter"/> implementation that actually writes to an <seealso cref="IndexWriter"/>.
    /// </summary>
    public class WriterWrapper : IWriter
    {
        private readonly IndexWriter _writer;
        private readonly ISupplier<IndexSearcher> _indexSearchSupplier;

        // for testing, can't make this accept an IndexWriter without making main constructor throw IOException
        public WriterWrapper(ISupplier<IndexWriter> writerFactory, ISupplier<IndexSearcher> indexSearcherSupplier)
        {
            _writer = writerFactory.Get();
            _indexSearchSupplier = indexSearcherSupplier;
        }

        public WriterWrapper(IIndexConfiguration configuration, UpdateMode mode,
            ISupplier<IndexSearcher> indexSearcherSupplier) : this(new ConfiguredIndexWriter(configuration, mode), indexSearcherSupplier)
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
                    // todo: uncomment until lucene 4.8
                    //var luceneConfig = writerSettings.GetWriterConfiguration(_configuration.Analyzer);
                    //return new IndexWriter(_configuration.Directory, luceneConfig);
                    return new IndexWriter(_configuration.Directory, _configuration.Analyzer, new IndexWriter.MaxFieldLength(writerSettings.MaxFieldLength));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public IWriter Get(UpdateMode mode)
        {
            throw new NotImplementedException();
        }

        public void AddDocuments(IEnumerable<Document> documents)
        {
            foreach (Document document in documents)
            {
                _writer.AddDocument(document);
            }
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
                documents.GetEnumerator().MoveNext();
                _writer.UpdateDocument(identifyingTerm, documents.First());
            }
            else
            {
                _writer.DeleteDocuments(identifyingTerm);
                foreach (Document document in documents)
                {
                    _writer.AddDocument(document);
                }
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

        public void Commit()
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

        public void Dispose()
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
                if (searcher != null)
                {
                    searcher.Dispose();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error closing: " + searcher, ex);
                //QuietLog.LogError(ex);
            }
        }
    }
}
