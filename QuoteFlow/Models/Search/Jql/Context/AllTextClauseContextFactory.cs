using System;
using System.Collections.Generic;
using QuoteFlow.Models.Assets.Search.Constants;
using QuoteFlow.Models.Assets.Search.Managers;
using QuoteFlow.Models.Search.Jql.Query.Clause;

namespace QuoteFlow.Models.Search.Jql.Context
{
    /// <summary>
    /// Calculates the context of the "all text" clause. Since the clause essentially aggregates all the free text fields
    /// visible to the user, the context is calculated by aggregating the contexts of each individual clause.
    /// </summary>
    public class AllTextClauseContextFactory : IClauseContextFactory
    {
        private readonly ISearchHandlerManager searchHandlerManager;

        public AllTextClauseContextFactory(ISearchHandlerManager searchHandlerManager)
        {
            if (searchHandlerManager == null)
            {
                throw new ArgumentNullException("searchHandlerManager");
            }

            this.searchHandlerManager = searchHandlerManager;
        }

        public virtual IClauseContext GetClauseContext(User searcher, ITerminalClause terminalClause)
        {
            IEnumerable<IClauseContextFactory> clauseContextFactories = GetFactories(searcher);
            ISet<IClauseContext> contexts = new HashSet<IClauseContext>();

            foreach (var factory in clauseContextFactories)
            {
                contexts.Add(factory.GetClauseContext(searcher, terminalClause));
            }

            return contexts.Union();
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
                new List<string>()
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