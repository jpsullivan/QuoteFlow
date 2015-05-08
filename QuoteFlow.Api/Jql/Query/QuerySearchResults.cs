using Jil;

namespace QuoteFlow.Api.Jql.Query
{
    public class QuerySearchResults
    {
        public QuerySearchResults()
        {
        }

        public QuerySearchResults(Searchers searchers, SearchRendererValueResults values)
        {
            Searchers = searchers;
            Values = values;
        }

        /// <summary>
        /// 
        /// </summary>
        [JilDirective("searchers")]
        public Searchers Searchers { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JilDirective("values")]
        public SearchRendererValueResults Values { get; set; }
    }
}