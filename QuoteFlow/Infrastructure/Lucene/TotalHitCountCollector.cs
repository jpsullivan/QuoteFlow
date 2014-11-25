using Lucene.Net.Index;
using Lucene.Net.Search;

namespace QuoteFlow.Infrastructure.Lucene
{
    /// <summary>
    /// Just counts the total number of hits.
    /// </summary>
    public class TotalHitCountCollector : Collector
    {
        /// <summary>
        /// Returns how many hits matched the search.
        /// </summary>
        public virtual int TotalHits { get; private set; }

        public override void SetScorer(Scorer scorer)
        {
        }

        public override void Collect(int doc)
        {
            TotalHits++;
        }

        public override void SetNextReader(IndexReader reader, int docBase)
        {
        }

        public override bool AcceptsDocsOutOfOrder
        {
            get { return true; }
        }
    }
}