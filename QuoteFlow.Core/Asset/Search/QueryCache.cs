using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Infrastructure.Caching;

namespace QuoteFlow.Core.Asset.Search
{
    public class QueryCache : IQueryCache
    {
        #region DI

        public ICacheService CacheService { get; protected set; }

        public QueryCache()
        {
        }

        public QueryCache(ICacheService cacheService)
        {
            CacheService = cacheService;
        }

        #endregion

        public bool? GetDoesQueryFitFilterFormCache(User searcher, IQuery query)
        {
            throw new NotImplementedException();
        }

        public void SetDoesQueryFitFilterFormCache(User searcher, IQuery query, bool doesItFit)
        {
            throw new NotImplementedException();
        }

        public IQueryContext GetQueryContextCache(User searcher, IQuery query)
        {
            IQueryContext context;
            var hasValue = GetQueryCache().TryGetValue(new QueryCacheKey(searcher, query), out context);
            return hasValue ? context : null;
        }

        public void SetQueryContextCache(User searcher, IQuery query, IQueryContext queryContext)
        {
            var queryContextCache = GetQueryCache();
            queryContextCache.Add(new QueryCacheKey(searcher, query), queryContext);
            //CacheService.SetItem(RequestCacheKeys.QueryContextCache, queryContextCache, TimeSpan.FromMinutes(1));
        }

        public IQueryContext GetSimpleQueryContextCache(User searcher, IQuery query)
        {
            IQueryContext context;
            var hasValue = GetExplicitQueryCache().TryGetValue(new QueryCacheKey(searcher, query), out context);
            return hasValue ? context : null;
        }

        public void SetSimpleQueryContextCache(User searcher, IQuery query, IQueryContext queryContext)
        {
            var explicitCache = GetExplicitQueryCache();
            explicitCache.Add(new QueryCacheKey(searcher, query), queryContext);
            CacheService.SetItem(RequestCacheKeys.SimpleQueryContextCache, explicitCache, TimeSpan.FromMinutes(1));
        }

        public List<IClauseHandler> GetClauseHandlers(User searcher, string jqlClauseName)
        {
            IEnumerable<IClauseHandler> handlers;
            if (GetClauseHandlerCache().TryGetValue(new QueryCacheClauseHandlerKey(searcher, jqlClauseName), out handlers))
            {
                return handlers.ToList();
            }

            return null;
        }

        public void SetClauseHandlers(User searcher, string jqlClauseName, ICollection<IClauseHandler> clauseHandlers)
        {
            var handlerCache = GetClauseHandlerCache();
            handlerCache.Add(new QueryCacheClauseHandlerKey(searcher, jqlClauseName), clauseHandlers);
            CacheService.SetItem(RequestCacheKeys.JqlClauseHandlerCache, handlerCache, TimeSpan.FromHours(1)); // todo cache check
        }

        public IList<QueryLiteral> GetValues(IQueryCreationContext context, IOperand operand, ITerminalClause jqlClause)
        {
            List<QueryLiteral> values;
            var hasValue = GetValueCache().TryGetValue(new QueryCacheLiteralsKey(context, operand, jqlClause), out values);
            return hasValue ? values : null;
        }

        public void SetValues(IQueryCreationContext context, IOperand operand, ITerminalClause jqlClause, List<QueryLiteral> values)
        {
            var valueCache = GetValueCache();
            valueCache.Add(new QueryCacheLiteralsKey(context, operand, jqlClause), values);
            //CacheService.SetItem(RequestCacheKeys.QueryLiteralsCache, valueCache, TimeSpan.FromMinutes(1));
        }

        public virtual IDictionary<QueryCacheKey, IQueryContext> GetQueryCache()
        {
            object queryCache = CacheService.GetItem(RequestCacheKeys.QueryContextCache);
            if (queryCache == null)
            {
                return new Dictionary<QueryCacheKey, IQueryContext>();
            }

            return queryCache as IDictionary<QueryCacheKey, IQueryContext>;
        }

        private IDictionary<QueryCacheKey, IQueryContext> GetExplicitQueryCache()
        {
            object explicitCache = CacheService.GetItem(RequestCacheKeys.SimpleQueryContextCache);
            if (explicitCache == null)
            {
                return new Dictionary<QueryCacheKey, IQueryContext>();
            }

            return explicitCache as IDictionary<QueryCacheKey, IQueryContext>;
        }

        private IDictionary<QueryCacheClauseHandlerKey, IEnumerable<IClauseHandler>> GetClauseHandlerCache()
        {
            object handlerCache = CacheService.GetItem(RequestCacheKeys.JqlClauseHandlerCache);
            if (handlerCache == null)
            {
                return new Dictionary<QueryCacheClauseHandlerKey, IEnumerable<IClauseHandler>>();
            }

            return handlerCache as IDictionary<QueryCacheClauseHandlerKey, IEnumerable<IClauseHandler>>;
        }

        private IDictionary<QueryCacheLiteralsKey, List<QueryLiteral>> GetValueCache()
        {
            object valueCache = CacheService.GetItem(RequestCacheKeys.QueryLiteralsCache);
            if (valueCache == null)
            {
                return new Dictionary<QueryCacheLiteralsKey, List<QueryLiteral>>();
            }

            return valueCache as IDictionary<QueryCacheLiteralsKey, List<QueryLiteral>>;
        } 

        /// <summary>
        /// A a key used for caching on Query and user pairs.
        /// </summary>
        public class QueryCacheKey
        {
            private readonly User _searcher;
            private readonly IQuery _query;

            public QueryCacheKey(User searcher, IQuery query)
            {
                if (query == null)
                {
                    throw new ArgumentNullException(nameof(query));
                }

                _searcher = searcher;
                _query = query;
            }

            public override bool Equals(object o)
            {
                if (this == o)
                {
                    return true;
                }
                if ((o == null) || (GetType() != o.GetType()))
                {
                    return false;
                }

                QueryCacheKey that = (QueryCacheKey) o;

                if (!_query.Equals(that._query))
                {
                    return false;
                }
                if (_searcher != null ? !_searcher.Equals(that._searcher) : that._searcher != null)
                {
                    return false;
                }

                return true;
            }

            public override int GetHashCode()
            {
                int result = _searcher != null ? _searcher.GetHashCode() : 0;
                result = 31 * result + _query.GetHashCode();
                return result;
            }
        }

        /// <summary>
		/// A a key used for caching on jqlClauseName and user pairs.
		/// </summary>
		internal class QueryCacheClauseHandlerKey
		{
            private readonly User _searcher;
            private readonly string _jqlClauseName;

			public QueryCacheClauseHandlerKey(User searcher, string jqlClauseName)
			{
			    if (jqlClauseName == null)
			    {
			        throw new ArgumentNullException(nameof(jqlClauseName));
			    }

				_searcher = searcher;
				_jqlClauseName = jqlClauseName;
			}

			public override bool Equals(object o)
			{
				if (this == o)
				{
					return true;
				}
				if ((o == null) || (GetType() != o.GetType()))
				{
					return false;
				}

			    QueryCacheClauseHandlerKey that = (QueryCacheClauseHandlerKey) o;

				if (!_jqlClauseName.Equals(that._jqlClauseName))
				{
					return false;
				}
				if (_searcher != null ?!_searcher.Equals(that._searcher) : that._searcher != null)
				{
					return false;
				}

				return true;
			}

			public override int GetHashCode()
			{
				int result = _searcher != null ? _searcher.GetHashCode() : 0;
				result = 31 * result + _jqlClauseName.GetHashCode();
				return result;
			}
		}

		/// <summary>
		/// A a key used for caching on Context, operand and TerminalClause triplets.
		/// </summary>
		private class QueryCacheLiteralsKey
		{
		    private readonly IQueryCreationContext _context;
		    private readonly IOperand _operand;
		    private readonly ITerminalClause _jqlClause;

			public QueryCacheLiteralsKey(IQueryCreationContext context, IOperand operand, ITerminalClause jqlClause)
			{
			    if (context == null)
			    {
			        throw new ArgumentNullException(nameof(context));
			    }

                if (operand == null)
			    {
			        throw new ArgumentNullException(nameof(operand));
			    }

                if (jqlClause == null)
			    {
			        throw new ArgumentNullException(nameof(jqlClause));
			    }

				_context = context;
				_operand = operand;
				_jqlClause = jqlClause;
			}

			public override bool Equals(object o)
			{
				if (this == o)
				{
					return true;
				}
				if ((o == null) || (GetType() != o.GetType()))
				{
					return false;
				}

				QueryCacheLiteralsKey that = (QueryCacheLiteralsKey) o;

				if (!_jqlClause.Equals(that._jqlClause))
				{
					return false;
				}
				if (!_operand.Equals(that._operand))
				{
					return false;
				}
				if (!_context.Equals(that._context))
				{
					return false;
				}

				return true;
			}

			public override int GetHashCode()
			{
				int result = _context.GetHashCode();
				result = 11 * result + _operand.GetHashCode();
				result = 31 * result + _jqlClause.GetHashCode();
				return result;
			}
		}
    }
}