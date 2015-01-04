using System;
using Lucene.Net.Search;
using QuoteFlow.Core.Util;

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
            helper = new DelayDisposableHelper(new CompositeDisposable(closeAction, new SearcherCloser(searcher)));
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
            internal readonly IndexSearcher Searcher;

            internal SearcherCloser(IndexSearcher searcher)
            {
                Searcher = searcher;
            }

            public void Dispose()
            {
                try
                {
                    Searcher.Dispose();
                    IncReaderRef();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            protected virtual void IncReaderRef()
            {
                Searcher.IndexReader.IncRef();
//                Counter searcherLuceneOpenInstrument = Instrumentation.pullCounter(InstrumentationName.SEARCHER_LUCENE_OPEN);
//                searcherLuceneOpenInstrument.IncrementAndGet();
            }

            protected virtual void DecReaderRef()
            {
                Searcher.IndexReader.DecRef();
//                Counter searcherLuceneCloseInstrument = Instrumentation.pullCounter(InstrumentationName.SEARCHER_LUCENE_CLOSE);
//                searcherLuceneCloseInstrument.IncrementAndGet();
            }
        }
    }
}
