using QuoteFlow.Models.Search.Jql.Query;

namespace QuoteFlow.Models.Search.Jql
{
    public class SearchResults
    {
        public SearchResults() { }

        public SearchResults(Searchers searchers, SearchRendererValueResults values)
        {
            Searchers = searchers;
            Values = values;
        }

        /// <summary>
        /// 
        /// </summary>
        public Searchers Searchers { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SearchRendererValueResults Values { get; set; }
    }
}