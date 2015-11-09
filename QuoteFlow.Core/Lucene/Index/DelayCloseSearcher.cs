using System;
using System.IO;
using System.Reactive.Disposables;
using Lucene.Net.Search;
using QuoteFlow.Api.Infrastructure.Helpers;

namespace QuoteFlow.Core.Lucene.Index
{
    public class DelayCloseSearcher : DelegateSearcher, IDelayDisposable
    {
        private readonly DelayDisposableHelper _helper;

        public DelayCloseSearcher(IndexSearcher searcher) 
            : base(Preconditions.CheckNotNull(searcher))
        {
            _helper = new DelayDisposableHelper(new SearcherCloser(searcher));
        }

        public DelayCloseSearcher(IndexSearcher searcher, Action closeAction) : base(searcher)
        {
            _helper = new DelayDisposableHelper(new CompositeDisposable(Disposable.Create(closeAction), new SearcherCloser(searcher)));
        }

        public void Open()
        {
            _helper.Open();
        }

        public void CloseWhenDone()
        {
            _helper.CloseWhenDone();
        }

        public bool IsClosed => _helper.IsClosed;

        protected override void Dispose(bool disposing)
        {
            _helper.Dispose();
        }

        /// <summary>
        /// Simple IDisposable adaptor for a Searcher.
        /// </summary>
        private sealed class SearcherCloser : IDisposable
        {
            private readonly IndexSearcher _searcher;

            internal SearcherCloser(IndexSearcher searcher)
            {
                _searcher = searcher;
                IncReaderRef();
            }

            public void Dispose()
            {
                try
                {
                    _searcher.Dispose();
                    DecReaderRef();
                }
                catch (IOException ex)
                {
                    throw ex;
                }
            }

            private void IncReaderRef()
            {
                _searcher.IndexReader.IncRef();
//                Counter searcherLuceneOpenInstrument = Instrumentation.pullCounter(InstrumentationName.SEARCHER_LUCENE_OPEN);
//                searcherLuceneOpenInstrument.IncrementAndGet();
            }

            private void DecReaderRef()
            {
                _searcher.IndexReader.DecRef();
//                Counter searcherLuceneCloseInstrument = Instrumentation.pullCounter(InstrumentationName.SEARCHER_LUCENE_CLOSE);
//                searcherLuceneCloseInstrument.IncrementAndGet();
            }
        }
    }
}
