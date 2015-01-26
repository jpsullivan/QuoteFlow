using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;

namespace QuoteFlow.Core.Jql.Query
{
    /// <summary>
    /// Able to map clauses to query handlers.
    /// </summary>
    public class QueryRegistry : IQueryRegistry
    {
        private readonly ISearchHandlerManager manager;

        public QueryRegistry(ISearchHandlerManager manager)
		{
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }
			this.manager = manager;
		}

        public ICollection<IClauseQueryFactory> GetClauseQueryFactory(IQueryCreationContext queryCreationContext, ITerminalClause clause)
        {
            if (clause == null)
            {
                throw new ArgumentNullException("clause");
            }

            ICollection<IClauseHandler> handlers;
            if (!queryCreationContext.SecurityOverriden)
            {
                handlers = manager.GetClauseHandler(queryCreationContext.User, clause.Name).ToList();
            }
            else
            {
                handlers = manager.GetClauseHandler(clause.Name).ToList();
            }

            var clauseQueryFactories = new List<IClauseQueryFactory>(handlers.Count);
            clauseQueryFactories.AddRange(handlers.Select(clauseHandler => clauseHandler.Factory));
            return clauseQueryFactories;
        }
    }
}