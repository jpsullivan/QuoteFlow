﻿using System;
using System.Collections.Generic;
using QuoteFlow.Models.Assets.Search.Constants;
using QuoteFlow.Models.Assets.Search.Managers;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Values;

namespace QuoteFlow.Models.Search.Jql.Context
{
    /// <summary>
    /// Calculates the context of the "all text" clause. Since the clause essentially aggregates all the free text fields
    /// visible to the user, the context is calculated by aggregating the contexts of each individual clause.
    /// </summary>
    /// <seealso cref= com.atlassian.jira.jql.query.AllTextClauseQueryFactory
    /// @since v4.0 </seealso>
    public class AllTextClauseContextFactory : IClauseContextFactory
    {
        private readonly CustomFieldManager customFieldManager;
        private readonly ISearchHandlerManager searchHandlerManager;
        private readonly ContextSetUtil contextSetUtil;

        public AllTextClauseContextFactory(CustomFieldManager customFieldManager, ISearchHandlerManager searchHandlerManager, ContextSetUtil contextSetUtil)
        {
            if (searchHandlerManager == null)
            {
                throw new ArgumentNullException("searchHandlerManager");
            }

            if (contextSetUtil == null)
            {
                throw new ArgumentNullException("contextSetUtil");
            }

            this.customFieldManager = customFieldManager;
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

            return contextSetUtil.union(contexts);
        }

        internal virtual IEnumerable<IClauseContextFactory> GetFactories(User searcher)
        {
            var factoryCollectionBuilder = new List<IClauseContextFactory>();

            factoryCollectionBuilder.AddRange(GetAllSystemFieldFactories(searcher));
            factoryCollectionBuilder.AddRange(GetAllCustomFieldFactories(searcher));

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

        internal virtual IEnumerable<IClauseContextFactory> GetAllCustomFieldFactories(User user)
        {
            var factories = new List<IClauseContextFactory>();
            var allCustomFields = customFieldManager.CustomFieldObjects;
            foreach (CustomField customField in allCustomFields)
            {
                CustomFieldSearcher searcher = customField.CustomFieldSearcher;
                if (searcher == null)
                {
                    continue;
                }

                CustomFieldSearcherClauseHandler fieldSearcherClauseHandler = searcher.CustomFieldSearcherClauseHandler;

                if (fieldSearcherClauseHandler == null || !fieldSearcherClauseHandler.SupportedOperators.contains(Operator.LIKE))
                {
                    continue;
                }

                if (!(fieldSearcherClauseHandler is AllTextCustomFieldSearcherClauseHandler))
                {
                    continue;
                }

                ICollection<ClauseHandler> handlers = searchHandlerManager.GetClauseHandler(user, customField.ClauseNames.PrimaryName);
                foreach (ClauseHandler handler in handlers)
                {
                    factories.Add(handler.ClauseContextFactory);
                }
            }
            return factories;
        }
    }

}