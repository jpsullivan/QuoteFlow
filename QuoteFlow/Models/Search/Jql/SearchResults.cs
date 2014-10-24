namespace QuoteFlow.Models.Search.Jql
{
    public class SearchResults
    {
        public SearchResults() { }

        public SearchResults(Query.Searchers searchers, SearchRendererValueResults values)
        {
            Searchers = searchers;
            Values = values;
        }

        /// <summary>
        /// 
        /// </summary>
        public Query.Searchers Searchers { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SearchRendererValueResults Values { get; set; }
    }
}