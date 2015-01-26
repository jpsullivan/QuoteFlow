using System;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Asset.Search
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