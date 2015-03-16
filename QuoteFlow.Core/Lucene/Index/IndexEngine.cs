using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using QuoteFlow.Api.Lucene.Index;
using QuoteFlow.Api.Util;
using QuoteFlow.Core.Index;

namespace QuoteFlow.Core.Lucene.Index
{
    /// <summary>
    /// Thread-safe container that manages our current <see cref="IndexSearcher"/> and <see cref="Writer"/>.
    /// 
    /// Gets passed searcher and writer factories that create new instances of these when required.
    /// </summary>
    public class IndexEngine : IEngine
    {
        private readonly WriterReference _writerReference;
        private readonly ISearcherFactory _searcherFactory;
        private readonly SearcherReference _searcherReference;
        private readonly FlushPolicy _writePolicy;
        private static IIndexConfiguration _configuration;

        /// <summary>
        /// Production constructor.
        /// </summary>
        /// <param name="configuration">The <see cref="Directory"/> and <see cref="Analyzer"/>.</param>
        /// <param name="writePolicy">When to flush writes.</param>
        public IndexEngine(IIndexConfiguration configuration, FlushPolicy writePolicy)
            : this(new SearcherFactory(configuration), null, configuration, writePolicy)
        {
        }

        public IndexEngine(ISearcherFactory searcherFactory, IWriter writerFactory, IIndexConfiguration configuration, FlushPolicy writePolicy)
        {
            _writePolicy = writePolicy;
            _configuration = configuration;
            _searcherFactory = searcherFactory;
            _searcherReference = new SearcherReference(searcherFactory);
            _writerReference = new WriterReference(writerFactory ?? new DefaultWriterFactory());
        }

        /// <summary>
        /// Wait until any open Searchers are closed and then close the connection.
        /// </summary>
        public void Dispose()
        {
            _writerReference.Dispose();
            _searcherReference.Dispose();
            _searcherFactory.Release();
        }

        public void Write(Operation operation)
        {
            try
            {
                _writePolicy.Perform(operation, _writerReference);
            }
            finally
            {
                _searcherReference.Dispose();
            }
        }

        public IndexSearcher Searcher
        {
            get
            {
                // mode is irrelevant to a Searcher
                return _searcherReference.Get(UpdateMode.Interactive);
            }
        }

        public void Clean()
        {
            Dispose();
            try
            {
                // todo: uncomment until lucene 4.8
//                IndexWriterConfig luceneConfig = new IndexWriterConfig(LuceneVersion.Get(), _configuration.Analyzer);
//                luceneConfig.OpenMode = IndexWriterConfig.OpenMode.CREATE;
//                new IndexWriter(_configuration.Directory, luceneConfig).Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// How to perform an actual write to the writer.
        /// </summary>
        public enum FlushPolicy
        {
            /// <summary>
            /// Do not flush or close.
            /// </summary>
            None,

            /// <summary>
            /// Commit the writers' pending updates, do not close.
            /// </summary>
            Flush,

            /// <summary>
            /// Close the writer after performing the write.
            /// </summary>
            Close
        }

        #region Searcher / Writer Reference

        /// <summary>
        /// "Thread-safe" holder of the current Searcher
        /// </summary>
        private class SearcherReference : ReferenceHolder<DelayCloseSearcher>
        {
            private readonly ISearcherFactory _searcherSupplier;

            public SearcherReference()
            {
            }

            internal SearcherReference(ISearcherFactory searcherSupplier)
            {
                if (searcherSupplier == null)
                {
                    throw new ArgumentNullException("searcherSupplier");
                }

                _searcherSupplier = searcherSupplier;
            }


            protected override void DoClose(DelayCloseSearcher element)
            {
                element.CloseWhenDone();
            }

            protected override DelayCloseSearcher Open(DelayCloseSearcher element)
            {
                element.Open();
                return element;
            }
        }

        /// <summary>
        /// "Thread-safe" holder of the current Writer.
        /// </summary>
        public class WriterReference : ReferenceHolder<IWriter>
        {
            private readonly IWriter _writerFactory;

            public WriterReference()
            {
            }

            internal WriterReference(IWriter writerFactory)
            {
                if (writerFactory == null)
                {
                    throw new ArgumentNullException("writerFactory");
                }

                _writerFactory = writerFactory;
            }

            public virtual void Commit()
            {
                if (!IsNull)
                {
                    Get().Commit();
                }
            }

            protected override void DoClose(IWriter element)
            {
                element.Dispose();
            }

            protected override IWriter Open(IWriter element)
            {
                return element;
            }
        }

        #endregion

        #region Writer Factory Implementation

        private class DefaultWriterFactory : IWriter
        {
            private readonly IndexSearcher _searcher;

            public DefaultWriterFactory()
            {
            }

            public DefaultWriterFactory(IndexSearcher searcher)
            {
                _searcher = searcher;
            }

            public IWriter Get(UpdateMode mode)
            {
                // be default, create a writer wrapper that has access to this engines searcher.
                return new WriterWrapper(_configuration, mode, new WriterFactorySupplier(_searcher));
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public void AddDocuments(IEnumerable<Document> documents)
            {
                throw new NotImplementedException();
            }

            public void DeleteDocuments(Term identifyingTerm)
            {
                throw new NotImplementedException();
            }

            public void UpdateDocuments(Term identifyingTerm, IEnumerable<Document> documents)
            {
                throw new NotImplementedException();
            }

            public void UpdateDocumentConditionally(Term identifyingTerm, Document document, string optimisticLockField)
            {
                throw new NotImplementedException();
            }

            public void Optimize()
            {
                throw new NotImplementedException();
            }

            public void Close()
            {
                throw new NotImplementedException();
            }

            public void Commit()
            {
                throw new NotImplementedException();
            }
        }

        private class WriterFactorySupplier : ISupplier<IndexSearcher>
        {
            private readonly IndexSearcher _searcher;

            public WriterFactorySupplier(IndexSearcher searcher)
            {
                _searcher = searcher;
            }

            public IndexSearcher Get()
            {
                return _searcher;
            }
        }

        #endregion

        public abstract class ReferenceHolder<T> : IDisposable
        {
            private readonly Lazy<T> _reference = new Lazy<T>(); 

            public void Dispose()
            {
                var supplier = _reference.Value;
                if (supplier != null)
                {
                    try
                    {
                        DoClose(supplier);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

            protected abstract void DoClose(T element);

            public T Get(UpdateMode mode)
            {
                while (true)
                {
//                    var reference = _reference;
//                    if (reference == null)
//                    {
//                        reference = new Lazy<T>();
//                    }

                    try
                    {
                        return Open(_reference.Value);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

            protected abstract T Open(T element);

            protected bool IsNull
            {
                get { return _reference.Value == null; }
            }

            protected T Get()
            {
                Lazy<T> lazyReference = _reference;
                return (lazyReference == null) ? default(T) : lazyReference.Value;
            }
        }

        public interface ISearcherFactory : ISupplier<IndexSearcher>
        {
            void Release();
        }

        private class SearcherFactory : ISearcherFactory
        {
            private IIndexConfiguration Configuration { get; set; }
            private volatile IndexReader _oldReader = null; // this is already held in the thread-safe SearcherReference

            public SearcherFactory(IIndexConfiguration configuration)
            {
                Configuration = configuration;
            }

            public IndexSearcher Get()
            {
                try
                {
                    IndexReader reader;
                    if (_oldReader != null)
                    {
                        try
                        {
                            reader = _oldReader.Reopen(true);
                            // If we actually get a new reader, we must dispose the old one.
                            if (reader != _oldReader)
                            {
                                // this will really dispose only when the ref count goes to zero
                                try
                                {
                                    _oldReader.Dispose();
                                }
                                catch (Exception ex)
                                {
                                    //log.debug("Tried to close an already closed reader.");
                                    throw ex;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Really this shouldn't happen unless someone closes the reader from outside all
                            // the inscrutable code in this class (and its friends) but
                            // don't worry, we will just open a new one in that case.
                            //log.warn("Tried to reopen the IndexReader, but it threw AlreadyClosedException. Opening a fresh IndexReader.");
                            reader = IndexReader.Open(Configuration.Directory, true);
                        }
                    }
                    else
                    {
                        reader = IndexReader.Open(Configuration.Directory, true);
                    }

                    _oldReader = reader;
                    return new IndexSearcher(reader);
                }
                catch (IOException ex)
                {
                    throw ex;
                }
            }

            public void Release()
            {
                IndexReader reader = _oldReader;
                if (reader == null) return;

                try
                {
                    reader.Dispose();
                    _oldReader = null;
                }
                catch (IOException ex)
                {
                    throw ex;
                }
            }
        }
    }

    public static class FlushPolicyExtensions
    {
        public static void Perform(this IndexEngine.FlushPolicy policy, Operation operation, IndexEngine.WriterReference writer)
        {
            try
            {
                operation.Perform(writer.Get(operation.Mode()));
            }
            finally
            {
                Commit(writer);
            }
        }

        private static void Commit(IndexEngine.WriterReference writer)
        {
            throw new NotImplementedException();
        }
    }
}