using System.Collections.Generic;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Core.Asset.Search;
using QuoteFlow.Core.Jql.Context;
using QuoteFlow.Core.Tests.Caching;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Context
{
    public class QueryCacheTests
    {
        public class SimpleContextCacheTests
        {
            [Fact]
            public void TestSimpleContextCache()
            {
                var cache = new Dictionary<QueryCache.QueryCacheKey, IQueryContext>();
                var queryCache = new QueryCacheTestImpl(cache);

                var searchQuery1 = new Api.Jql.Query.Query(new TerminalClause("1", Operator.EQUALS, "1"));
                var searchQuery2 = new Api.Jql.Query.Query(new TerminalClause("2", Operator.EQUALS, "2"));
                var searchQuery3 = new Api.Jql.Query.Query(new TerminalClause("3", Operator.EQUALS, "3"));

                var queryContext1 = new QueryContext(new ClauseContext(new List<ICatalogManufacturerContext> 
                {
                    new CatalogManufacturerContext(new CatalogContext(10), AllManufacturersContext.Instance)
                }));
                var queryContext2 = new QueryContext(new ClauseContext(new List<ICatalogManufacturerContext> 
                {
                    new CatalogManufacturerContext(new CatalogContext(15), AllManufacturersContext.Instance)
                }));

                queryCache.SetSimpleQueryContextCache(null, searchQuery1, queryContext1);
                queryCache.SetSimpleQueryContextCache(null, searchQuery2, queryContext2);

                Assert.Equal(queryContext1, queryCache.GetSimpleQueryContextCache(null, searchQuery1));
                Assert.Equal(queryContext2, queryCache.GetSimpleQueryContextCache(null, searchQuery2));
                Assert.Null(queryCache.GetQueryContextCache(null, searchQuery3));
            }

            private class QueryCacheTestImpl : MockQueryCache
            {
                private readonly IDictionary<QueryCacheKey, IQueryContext> _cache;

                public QueryCacheTestImpl(IDictionary<QueryCacheKey, IQueryContext> cache)
		        {
			        _cache = cache;
		        }

                public override IDictionary<QueryCacheKey, IQueryContext> GetQueryCache()
                {
                    return _cache;
                }
            }
        }
    }
}