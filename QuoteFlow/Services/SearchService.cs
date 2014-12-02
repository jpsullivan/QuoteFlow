using System;
using System.Threading.Tasks;
using QuoteFlow.Infrastructure.Util;
using QuoteFlow.Models;
using QuoteFlow.Models.Assets.Search;
using QuoteFlow.Models.Assets.Search.Managers;
using QuoteFlow.Models.Search;
using QuoteFlow.Models.Search.Jql.Context;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Parser;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Util;
using QuoteFlow.Models.Search.Jql.Validator;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Services
{
    public class SearchService : ISearchService
    {
        public IJqlQueryParser JqlQueryParser { get; protected set; }
        public IJqlStringSupport JqlStringSupport { get; protected set; }
        public IJqlOperandResolver JqlOperandResolver { get; protected set; }
        public ValidatorVisitor.ValidatorVisitorFactory ValidatorVisitorFactory { get; protected set; }
        public ISearchHandlerManager SearchHandlerManager { get; protected set; }
        public QueryContextVisitor.QueryContextVisitorFactory QueryContextVisitorFactory { get; protected set; }
        public IQueryCache QueryCache { get; protected set; } // request-level cache, not persistent
        public ISearchProvider SearchProvider { get; protected set; }

        public SearchService(IJqlQueryParser jqlQueryParser, IJqlStringSupport jqlStringSupport, IJqlOperandResolver jqlOperandResolver, ValidatorVisitor.ValidatorVisitorFactory validatorVisitorFactory, ISearchHandlerManager searchHandlerManager, QueryContextVisitor.QueryContextVisitorFactory queryContextVisitorFactory, IQueryCache queryCache, ISearchProvider searchProvider)
        {
            JqlQueryParser = jqlQueryParser;
            JqlStringSupport = jqlStringSupport;
            JqlOperandResolver = jqlOperandResolver;
            ValidatorVisitorFactory = validatorVisitorFactory;
            SearchHandlerManager = searchHandlerManager;
            QueryContextVisitorFactory = queryContextVisitorFactory;
            QueryCache = queryCache;
            SearchProvider = searchProvider;
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
                return new QueryContext(ClauseContext.CreateGlobalClauseContext());
            }
            
            IQueryContext queryContext = QueryCache.GetQueryContextCache(searcher, query);
            if (queryContext == null)
            {
                // calculate both the full and simple contexts and cache them
                var factory = new QueryContextVisitor.QueryContextVisitorFactory(SearchHandlerManager);
                QueryContextVisitor visitor = factory.CreateVisitor(searcher);

                QueryContextVisitor.ContextResult result = clause.Accept(visitor);
                queryContext = new QueryContext(result.FullContext);
                var explicitQueryContext = new QueryContext(result.SimpleContext);
                QueryCache.SetQueryContextCache(searcher, query, queryContext);
                QueryCache.SetSimpleQueryContextCache(searcher, query, explicitQueryContext);
            }
            return queryContext;
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