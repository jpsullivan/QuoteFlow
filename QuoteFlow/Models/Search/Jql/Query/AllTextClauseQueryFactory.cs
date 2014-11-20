using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Models.Assets.Search.Constants;
using QuoteFlow.Models.Assets.Search.Managers;
using QuoteFlow.Models.Search.Jql.Clauses;
using QuoteFlow.Models.Search.Jql.Query.Clause;

namespace QuoteFlow.Models.Search.Jql.Query
{
    /// <summary>
    /// Builds lucene queries for the "all text" clause. This clause aggregates all the free text fields in the system which
    /// the user can see (or simply all fields if we are overriding security). Therefore, we acquire the appropriate clause
    /// query factories for these fields, collect their individual results, then aggregate them with SHOULDs.
    /// <p>
    /// For example, the query <code>text ~ "john"</code> is equivalent to <code>summary ~ "john" OR description ~ "john" OR ...</code>.
    /// </summary>
    public class AllTextClauseQueryFactory : ClauseQueryFactory
    {
        private readonly ISearchHandlerManager searchHandlerManager;

        public AllTextClauseQueryFactory(ISearchHandlerManager searchHandlerManager)
        {
            if (searchHandlerManager == null)
            {
                throw new ArgumentNullException("searchHandlerManager");
            }

            this.searchHandlerManager = searchHandlerManager;
        }

        public virtual QueryFactoryResult GetQuery(QueryCreationContext queryCreationContext, ITerminalClause terminalClause)
        {
            Operator @operator = terminalClause.Operator;

            if (@operator != Operator.LIKE)
            {
                return QueryFactoryResult.CreateFalseResult();
            }

            IList<IClauseQueryFactory> factories = GetFactories(queryCreationContext);
            IList<QueryFactoryResult> results = new List<QueryFactoryResult>(factories.Count);
            foreach (IClauseQueryFactory clauseQueryFactory in factories)
            {
                results.Add(clauseQueryFactory.GetQuery(queryCreationContext, terminalClause));
            }

            return QueryFactoryResult.MergeResultsWithShould(results); // TODO CJM kickass changes this
        }

        internal virtual IList<IClauseQueryFactory> GetFactories(IQueryCreationContext queryCreationContext)
        {
            var factoryCollectionBuilder = new List<IClauseQueryFactory>();

            factoryCollectionBuilder.AddRange(GetAllSystemFieldFactories(queryCreationContext));

            return factoryCollectionBuilder;
        }

        internal virtual IEnumerable<IClauseQueryFactory> GetAllSystemFieldFactories(IQueryCreationContext queryCreationContext)
        {
            var factories = new List<IClauseQueryFactory>();
            var systemFieldClauseNames = new List<string>
            {
                SystemSearchConstants.ForComments().JqlClauseNames.PrimaryName,
                SystemSearchConstants.ForDescription().JqlClauseNames.PrimaryName,
                SystemSearchConstants.ForSummary().JqlClauseNames.PrimaryName
            };

            foreach (string clauseName in systemFieldClauseNames)
            {
                var handlers = GetHandlersForClauseName(queryCreationContext, clauseName);
                factories.AddRange(handlers.Select(handler => handler.Factory));
            }

            return factories;
        }

        private IEnumerable<IClauseHandler> GetHandlersForClauseName(IQueryCreationContext queryCreationContext, string primaryClauseName)
        {
            if (queryCreationContext.SecurityOverriden)
            {
                return searchHandlerManager.GetClauseHandler(primaryClauseName);
            }
            return searchHandlerManager.GetClauseHandler(queryCreationContext.User, primaryClauseName);
        }
    }

}