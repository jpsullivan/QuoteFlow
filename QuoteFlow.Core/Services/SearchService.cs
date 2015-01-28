using System;
using System.Threading.Tasks;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Parser;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Util;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Search;
using QuoteFlow.Api.Services;
using QuoteFlow.Api.Util;
using QuoteFlow.Core.Asset.Search;
using QuoteFlow.Core.Jql.Context;
using QuoteFlow.Core.Jql.Validator;
using ClauseContext = QuoteFlow.Api.Jql.Context.ClauseContext;

namespace QuoteFlow.Core.Services
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

        public ISearchContext GetSearchContext(User searcher, IQuery query)
        {
            if (query != null)
            {
                var queryContext = GetSimpleQueryContext(searcher, query);
                if (queryContext != null)
                {
                    var searchContext = QueryContextCon
                }
            }
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