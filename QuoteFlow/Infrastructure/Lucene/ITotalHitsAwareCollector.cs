using Lucene.Net.Search;
using QuoteFlow.Infrastructure.Paging;

namespace QuoteFlow.Infrastructure.Lucene
{
    /// <summary>
    /// Interface to allow classes that extend <see cref="Collector"/> to be informed of the total number of hits.
    /// If you wish to search for the top 500 results, but also know the total hits.
    /// 
    /// This will call setTotalHits(int x) on your collector.
    /// </summary>
    public interface ITotalHitsAwareCollector
    {
        /// <summary>
        /// Set the total hits.
        /// This may be larger than <see cref="PagerFilter{T}.Max"/> requested when the search is invoked.
        /// There is no ordering guarenteed between calls to this method and calls to <see cref="Collector.Collect(int)"/>.
        /// This method will be called even if collect() is not (e.g. there are no results).
        /// </summary>
        int TotalHits { set; }
    }
}