using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Infrastructure.Paging;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Parser;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Order;
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
        #region DI

        public IJqlQueryParser JqlQueryParser { get; protected set; }
        public IJqlStringSupport JqlStringSupport { get; protected set; }
        public IJqlOperandResolver JqlOperandResolver { get; protected set; }
        public ValidatorVisitor.ValidatorVisitorFactory ValidatorVisitorFactory { get; protected set; }
        public ISearchHandlerManager SearchHandlerManager { get; protected set; }
        public QueryContextVisitor.QueryContextVisitorFactory QueryContextVisitorFactory { get; protected set; }
        protected QueryContextConverter QueryContextConverter { get; set; }
        public IQueryCache QueryCache { get; protected set; } // request-level cache, not persistent
        public ISearchProvider SearchProvider { get; protected set; }

        protected SearchService()
        {
        }

        public SearchService(IJqlQueryParser jqlQueryParser, IJqlStringSupport jqlStringSupport, 
            IJqlOperandResolver jqlOperandResolver, ValidatorVisitor.ValidatorVisitorFactory validatorVisitorFactory, 
            ISearchHandlerManager searchHandlerManager, QueryContextVisitor.QueryContextVisitorFactory queryContextVisitorFactory,
            QueryContextConverter queryContextConverter, IQueryCache queryCache, ISearchProvider searchProvider)
        {
            JqlQueryParser = jqlQueryParser;
            JqlStringSupport = jqlStringSupport;
            JqlOperandResolver = jqlOperandResolver;
            ValidatorVisitorFactory = validatorVisitorFactory;
            SearchHandlerManager = searchHandlerManager;
            QueryContextConverter = queryContextConverter;
            QueryContextVisitorFactory = queryContextVisitorFactory;
            QueryCache = queryCache;
            SearchProvider = searchProvider;
        }

        #endregion

        public Task<SearchResult> Search(SearchFilter filter)
        {
            throw new NotImplementedException();
        }

        public SearchResults Search(User searcher, IQuery query, IPagerFilter pager)
        {
            return SearchProvider.Search(query, searcher, pager);
        }

        public long SearchCount(User searcher, IQuery query)
        {
            return SearchProvider.SearchCount(query, searcher);
        }

        public string GetQueryString(User searcher, IQuery query)
        {
            return QueryString.Current.Add("jqlQuery", GetJqlString(query)).ToString();
        }

        public ParseResult ParseQuery(User searcher, string query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            IQuery newQuery = null;
            var errors = new MessageSet();

            try
            {
                newQuery = JqlQueryParser.ParseQuery(query);
            }
            catch (JqlParseException ex)
            {
                var errorMessage = ex.ParseErrorMessage;
                errors.AddErrorMessage(errorMessage.ToString());
            }

            return new ParseResult(newQuery, errors);
        }

        public IMessageSet ValidateQuery(User searcher, IQuery query)
        {
            return ValidateQuery(searcher, query, null);
        }

        public IMessageSet ValidateQuery(User searcher, IQuery query, long? searchRequestId)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            var clause = query.WhereClause;
            IMessageSet messageSet;

            if (clause != null)
            {
                // validate clause
                var visitor = ValidatorVisitorFactory.CreateVisitor(searcher, searchRequestId);
                messageSet = clause.Accept(visitor);
            }
            else
            {
                messageSet = new MessageSet();
            }

            var orderBy = query.OrderByClause;
            if (orderBy != null)
            {
                // validate OrderBy
                //messageSet.AddMessageSet(OrderByValidator.Validate(searcher, orderBy));
            }

            return messageSet;
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
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            // We know that the read and put are non-atomic but we will always generate 
            // the same result and it does not matter if two threads over-write the result into the cache.
            IClause clause = query.WhereClause;
            if (clause == null)
            {
                // return the ALL-ALL context for the all query
                return new QueryContext(ClauseContext.CreateGlobalClauseContext());
            }
            
            IQueryContext simpleQueryContext = QueryCache.GetSimpleQueryContextCache(searcher, query);
            
            if (simpleQueryContext != null) return simpleQueryContext;

            // calculate both the full and simple contexts and cache them again
            QueryContextVisitor visitor = QueryContextVisitorFactory.CreateVisitor(searcher);
            QueryContextVisitor.ContextResult result = clause.Accept(visitor);
            simpleQueryContext = new QueryContext(result.SimpleContext);
            QueryContext fullQueryContext = new QueryContext(result.FullContext);
            QueryCache.SetQueryContextCache(searcher, query, fullQueryContext);
            QueryCache.SetSimpleQueryContextCache(searcher, query, simpleQueryContext);

            return simpleQueryContext;
        }

        public ISearchContext GetSearchContext(User searcher, IQuery query)
        {
            if (query != null)
            {
                var queryContext = GetSimpleQueryContext(searcher, query);
                if (queryContext != null)
                {
                    var searchContext = QueryContextConverter.GetSearchContext(queryContext);
                    if (searchContext != null)
                    {
                        return searchContext;
                    }
                }
            }

            // Could not generate one so lets return an empty one
            return CreateSearchContext(new List<int?>(), new List<int>());
        }

        public string GetJqlString(IQuery query)
        {
            if (query == null) throw new ArgumentNullException("query");

            if (query.QueryString != null)
            {
                return query.QueryString;
            }

            return GetGeneratedJqlString(query);
        }

        public string GetGeneratedJqlString(IQuery query)
        {
            if (query == null) throw new ArgumentNullException("query");

            return JqlStringSupport.GenerateJqlString(query);
        }

        public ISearchContext CreateSearchContext(List<int?> catalogs, List<int> manufacturers)
        {
            return new SearchContext(catalogs, manufacturers);
        }
    }
}