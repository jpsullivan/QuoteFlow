using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Search;

namespace QuoteFlow.Core.Lucene.Index
{
    internal class DelayCloseSearcher : DelegateSearcher, IDelayDisposable
    {
        private readonly DelayDisposableHelper helper;

        internal DelayCloseSearcher(IndexSearcher searcher) : base(searcher)
        {
            helper = new DelayDisposableHelper(new SearcherCloser(searcher));
        }

        internal DelayCloseSearcher(IndexSearcher searcher, IDisposable closeAction) : base(searcher)
        {
            helper = new DelayDisposableHelper(new CompositeCloseable(closeAction, new SearcherCloser(searcher)));
        }

        public void Open()
        {
            helper.Open();
        }

        public void CloseWhenDone()
        {
            helper.CloseWhenDone();
        }

        public bool IsClosed
        {
            get { return helper.IsClosed; }
        }

        public new void Dispose()
        {
            helper.Dispose();
        }

        private class SearcherCloser : IDisposable
        {
            internal readonly IndexSearcher searcher;

            internal SearcherCloser(IndexSearcher searcher)
            {
                this.searcher = searcher;
            }

            public void Dispose()
            {
                try
                {
                    searcher.Dispose();
                    IncReaderRef();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            protected virtual void IncReaderRef()
            {
                searcher.IndexReader.IncRef();
                Counter searcherLuceneOpenInstrument = Instrumentation.pullCounter(InstrumentationName.SEARCHER_LUCENE_OPEN);
                searcherLuceneOpenInstrument.IncrementAndGet();
            }

            protected virtual void DecReaderRef()
            {
                searcher.IndexReader.DecRef();
                Counter searcherLuceneCloseInstrument = Instrumentation.pullCounter(InstrumentationName.SEARCHER_LUCENE_CLOSE);
                searcherLuceneCloseInstrument.IncrementAndGet();
            }
        }
    }
}
