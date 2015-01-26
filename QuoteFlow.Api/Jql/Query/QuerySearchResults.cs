namespace QuoteFlow.Api.Jql.Query
{
    public class QuerySearchResults
    {
        public QuerySearchResults() { }

        public QuerySearchResults(Searchers searchers, SearchRendererValueResults values)
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