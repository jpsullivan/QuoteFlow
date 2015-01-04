using System;
using System.Collections.Generic;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Core.Lucene.Index
{
    /// <summary>
    /// Override all searcher methods and delegate to another <seealso cref="IndexSearcher"/>.
    /// 
    /// Note, this is a fragile extension. We need to check each time we update Lucene that new superclass
    /// methods have been added and override them too!
    /// </summary>
    internal class DelegateSearcher : IndexSearcher, ISupplier<IndexSearcher>
    {
        private readonly IndexSearcher _searcher;

        internal DelegateSearcher(IndexSearcher searcher) : base(searcher.IndexReader)
        {
            if (searcher == null)
            {
                throw new ArgumentNullException("searcher");
            }

            _searcher = searcher;
        }

        public virtual IndexSearcher Get()
        {
            return _searcher;
        }

        public override IndexReader IndexReader
        {
            get { return _searcher.IndexReader; }
        }

        protected override void GatherSubReaders(IList<IndexReader> allSubReaders, IndexReader reader)
        {
            // Note: cannot call protected method. Check super to make sure it calls back using public methods only.
            base.GatherSubReaders(allSubReaders, reader);
        }

        public override IndexReader[] SubReaders
        {
            get { return _searcher.SubReaders; }
        }

        public override TopFieldDocs Search(Weight weight, Filter filter, int nDocs, Sort sort, bool fillFields)
        {
            throw new NotSupportedException("We Cannot delegate this protected method.");
        }

        public override void SetDefaultFieldSortScoring(bool doTrackScores, bool doMaxScore)
        {
            _searcher.SetDefaultFieldSortScoring(doTrackScores, doMaxScore);
        }

        protected override void Dispose(bool disposing)
        {
            _searcher.Dispose();
        }

        public override Document Doc(int i)
        {
            return _searcher.Doc(i);
        }

        public override Document Doc(int i, FieldSelector fieldSelector)
        {
            return _searcher.Doc(i, fieldSelector);
        }

        public override int DocFreq(Term term)
        {
            return _searcher.DocFreq(term);
        }

        public override int[] DocFreqs(Term[] terms)
        {
            return _searcher.DocFreqs(terms);
        }

        public override Explanation Explain(Query query, int doc)
        {
            return _searcher.Explain(query, doc);
        }

        public override Explanation Explain(Weight weight, int doc)
        {
            return _searcher.Explain(weight, doc);
        }

        public override Similarity Similarity
        {
            get { return _searcher.Similarity; }
            set { _searcher.Similarity = value; }
        }

        public override int MaxDoc
        {
            get { return _searcher.MaxDoc; }
        }

        public override Query Rewrite(Query query)
        {
            return _searcher.Rewrite(query);
        }

        public override void Search(Query query, Filter filter, Collector results)
        {
            _searcher.Search(query, filter, results);
        }

        public override TopFieldDocs Search(Query query, int i, Sort sort)
        {
            return _searcher.Search(query, i, sort);
        }

        public override TopFieldDocs Search(Query query, Filter filter, int i, Sort sort)
        {
            return _searcher.Search(query, filter, i, sort);
        }

        public override TopDocs Search(Query query, int i)
        {
            return _searcher.Search(query, i);
        }

        public override TopDocs Search(Query query, Filter filter, int i)
        {
            return _searcher.Search(query, filter, i);
        }

        public override void Search(Query query, Collector results)
        {
            _searcher.Search(query, results);
        }

        public override void Search(Weight weight, Filter filter, Collector results)
        {
            _searcher.Search(weight, filter, results);
        }

        public override TopFieldDocs Search(Weight weight, Filter filter, int i, Sort sort)
        {
            return _searcher.Search(weight, filter, i, sort);
        }

        public override TopDocs Search(Weight weight, Filter filter, int i)
        {
            return _searcher.Search(weight, filter, i);
        }

        public override bool Equals(object obj)
        {
            return _searcher.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _searcher.GetHashCode();
        }
    }
}
