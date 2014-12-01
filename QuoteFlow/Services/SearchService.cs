using System;
using System.Threading.Tasks;
using QuoteFlow.Infrastructure.Util;
using QuoteFlow.Models;
using QuoteFlow.Models.Assets.Search;
using QuoteFlow.Models.Search;
using QuoteFlow.Models.Search.Jql.Context;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Parser;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Util;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Services
{
    public class SearchService : ISearchService
    {
        public IJqlQueryParser JqlQueryParser { get; set; }
        public IJqlStringSupport JqlStringSupport { get; set; }
        public IJqlOperandResolver JqlOperandResolver { get; set; }

        public SearchService(
            IJqlQueryParser jqlQueryParser, 
            IJqlStringSupport jqlStringSupport, 
            IJqlOperandResolver jqlOperandResolver)
        {
            JqlQueryParser = jqlQueryParser;
            JqlStringSupport = jqlStringSupport;
            JqlOperandResolver = jqlOperandResolver;
        }

        public Task<SearchResult> Search(SearchFilter filter)
        {
            throw new NotImplementedException();
        }

        public SearchResults Search(User searcher, IQuery query)
        {
            throw new NotImplementedException();
        }

        public long SearchCount(User searcher, IQuery query)
        {
            throw new NotImplementedException();
        }

        public string GetQueryString(User searcher, string query)
        {
            throw new NotImplementedException();
        }

        public ParseResult ParseQuery(User searcher, string query)
        {
            throw new NotImplementedException();
        }

        public IMessageSet ValidateQuery(User searcher, IQuery query)
        {
            throw new NotImplementedException();
        }

        public IMessageSet ValidateQuery(User searcher, IQuery query, long searchRequestId)
        {
            throw new NotImplementedException();
        }

        public IQueryContext GetQueryContext(User searcher, IQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            // We know that the read and put are non-atomic but we will always generate the same result and it does not
            // matter if two threads over-write the result into the cache
            IClause clause = query.WhereClause;
            if (clause == null)
            {
                // return the ALL-ALL context for the all query
                return new QueryContext(ClauseContextImpl.createGlobalClauseContext());
            }
            else
            {
                QueryContext queryContext = queryCache.getQueryContextCache(searcher, query);
                if (queryContext == null)
                {
                    // calculate both the full and simple contexts and cache them
                    QueryContextVisitor visitor = queryContextVisitorFactory.createVisitor(searcher);
                    QueryContextVisitor.ContextResult result = clause.accept(visitor);
                    queryContext = new QueryContextImpl(result.FullContext);
                    QueryContext explicitQueryContext = new QueryContextImpl(result.SimpleContext);
                    queryCache.setQueryContextCache(searcher, query, queryContext);
                    queryCache.setSimpleQueryContextCache(searcher, query, explicitQueryContext);
                }
                return queryContext;
            }

            throw new NotImplementedException();
        }

        public IQueryContext GetSimpleQueryContext(User searcher, IQuery query)
        {
            throw new NotImplementedException();
        }

        public SearchContext GetSearchContext(User searcher, IQuery query)
        {
            throw new NotImplementedException();
        }

        public string GetJqlString(IQuery query)
        {
            throw new NotImplementedException();
        }

        public string GetGeneratedJqlString(IQuery query)
        {
            throw new NotImplementedException();
        }
    }
}