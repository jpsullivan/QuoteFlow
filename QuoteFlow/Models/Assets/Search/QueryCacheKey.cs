using System;
using QuoteFlow.Models.Search.Jql.Query;

namespace QuoteFlow.Models.Assets.Search
{
    /// <summary>
    /// A a key used for caching on Query and user pairs.
    /// </summary>
    public class QueryCacheKey
    {
        private readonly User searcher;
        private readonly IQuery query;

        public QueryCacheKey(User searcher, IQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            this.searcher = searcher;
            this.query = query;
        }
    }
}