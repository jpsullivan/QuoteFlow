using System;
using System.Collections.Generic;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Jql.Context
{
    /// <summary>
    /// Calculates the context of the "all text" clause. Since the clause essentially aggregates all the free text fields
    /// visible to the user, the context is calculated by aggregating the contexts of each individual clause.
    /// </summary>
    public class AllTextClauseContextFactory : IClauseContextFactory
    {
        private readonly ISearchHandlerManager searchHandlerManager;
        private readonly ContextSetUtil contextSetUtil;

        public AllTextClauseContextFactory(ISearchHandlerManager searchHandlerManager, ContextSetUtil contextSetUtil)
        {
            if (searchHandlerManager == null) throw new ArgumentNullException(nameof(searchHandlerManager));
            if (contextSetUtil == null) throw new ArgumentNullException(nameof(contextSetUtil));

            this.searchHandlerManager = searchHandlerManager;
            this.contextSetUtil = contextSetUtil;
        }

        public virtual IClauseContext GetClauseContext(User searcher, ITerminalClause terminalClause)
        {
            IEnumerable<IClauseContextFactory> clauseContextFactories = GetFactories(searcher);
            ISet<IClauseContext> contexts = new HashSet<IClauseContext>();

            foreach (var factory in clauseContextFactories)
            {
                contexts.Add(factory.GetClauseContext(searcher, terminalClause));
            }

            return contextSetUtil.Union(contexts);
        }

        internal virtual IEnumerable<IClauseContextFactory> GetFactories(User searcher)
        {
            var factoryCollectionBuilder = new List<IClauseContextFactory>();

            factoryCollectionBuilder.AddRange(GetAllSystemFieldFactories(searcher));

            return factoryCollectionBuilder;
        }

        internal virtual IEnumerable<IClauseContextFactory> GetAllSystemFieldFactories(User searcher)
        {
            var factories = new List<IClauseContextFactory>();
            var systemFieldClauseNames =
                new List<string>
                {
                    SystemSearchConstants.ForComments().JqlClauseNames.PrimaryName,
                    SystemSearchConstants.ForDescription().JqlClauseNames.PrimaryName,
                    //SystemSearchConstants.forEnvironment().JqlClauseNames.PrimaryName,
                    SystemSearchConstants.ForSummary().JqlClauseNames.PrimaryName
                };

            foreach (string clauseName in systemFieldClauseNames)
            {
                var handlers = searchHandlerManager.GetClauseHandler(searcher, clauseName);
                foreach (var handler in handlers)
                {
                    factories.Add(handler.ClauseContextFactory);
                }
            }

            return factories;
        }
    }
}