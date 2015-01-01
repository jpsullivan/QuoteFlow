using System;
using Lucene.Net.Search;
using QuoteFlow.Api.Lucene.Index;

namespace QuoteFlow.Core.Lucene.Index
{
    /// <summary>
    /// Thread-safe container that manages our current <see cref="IndexSearcher"/> and <see cref="Writer"/>.
    /// 
    /// Gets passed searcher and writer factories that create new instances of these when required.
    /// </summary>
    public class IndexEngine : IEngine
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Write(IndexOperation operation)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public IndexSearcher Searcher
        {
            get { throw new NotImplementedException(); }
        }

        public void Clean()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// How to perform an actual write to the writer.
        /// </summary>
        internal enum FlushPolicy
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

        internal abstract class ReferenceHolder<T> : IDisposable
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
                    var reference = _reference.Value;
                    if (reference == null)
                    {
                        reference = new Lazy<T>;
                    }
                }
            }
        }
    }
}