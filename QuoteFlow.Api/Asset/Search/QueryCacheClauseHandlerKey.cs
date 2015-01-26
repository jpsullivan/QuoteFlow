using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Asset.Search
{
    /// <summary>
    /// A a key used for caching on jqlClauseName and user pairs.
    /// </summary>
    internal class QueryCacheClauseHandlerKey
    {
        private readonly User searcher;
        private readonly string jqlClauseName;

        public QueryCacheClauseHandlerKey(User searcher, string jqlClauseName)
        {
            this.searcher = searcher;
            this.jqlClauseName = jqlClauseName;
        }
    }
}