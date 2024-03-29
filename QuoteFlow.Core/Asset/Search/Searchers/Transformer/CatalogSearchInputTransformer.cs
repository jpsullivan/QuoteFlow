﻿using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Search.Searchers.Transformer;
using QuoteFlow.Api.Asset.Search.Searchers.Util;
using QuoteFlow.Api.Asset.Transport;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;
using QuoteFlow.Api.Util;
using QuoteFlow.Core.Jql.Resolver;

namespace QuoteFlow.Core.Asset.Search.Searchers.Transformer
{
    /// <summary>
    /// The SearchInputTransformer for the Catalog system field.
    /// </summary>
    public class CatalogSearchInputTransformer : ISearchInputTransformer
    {
        private readonly IFieldFlagOperandRegistry _fieldFlagOperandRegistry;
        private readonly IJqlOperandResolver _operandResolver;
        private readonly CatalogIndexInfoResolver _projectIndexInfoResolver;
        private readonly ICatalogService _catalogService;

        public CatalogSearchInputTransformer(CatalogIndexInfoResolver projectIndexInfoResolver, IJqlOperandResolver operandResolver, 
            IFieldFlagOperandRegistry fieldFlagOperandRegistry, ICatalogService catalogService)
        {
            _fieldFlagOperandRegistry = fieldFlagOperandRegistry;
            _operandResolver = operandResolver;
            _projectIndexInfoResolver = projectIndexInfoResolver;
            _catalogService = catalogService;
        }

        public virtual void PopulateFromParams(User user, IFieldValuesHolder fieldValuesHolder, IActionParams actionParams)
        {
            string url = SystemSearchConstants.ForCatalog().UrlParameter;
            var @params = actionParams.GetValuesForKey(url);
            fieldValuesHolder.Add(url, @params == null ? null : new List<string>(@params));
        }

        public void ValidateParams(User searcher, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder, IErrorCollection errors)
        {
            // We currently dont do anything
        }

        public virtual void PopulateFromQuery(User user, IFieldValuesHolder fieldValuesHolder, IQuery query, ISearchContext searchContext)
        {
            var uncleanedValues = GetNavigatorValuesAsStrings(user, query);

            IList<string> values = new List<string>(uncleanedValues);

            // null swap all of the values
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] == null)
                {
                    values[i] = "-1";
                }
            }

            fieldValuesHolder.Add(SystemSearchConstants.ForCatalog().UrlParameter, values);
            CatalogIdInSession = uncleanedValues;
        }

        public virtual bool DoRelevantClausesFitFilterForm(User user, IQuery query, ISearchContext searchContext)
        {
            return true;
        }

        public virtual ISet<string> GetIdValuesAsStrings(User searcher, Query query)
        {
            var helper = CreateIndexedInputHelper();
            return helper.GetAllIndexValuesForMatchingClauses(searcher, SystemSearchConstants.ForCatalog().JqlClauseNames, query);
        }

        private ISet<string> GetNavigatorValuesAsStrings(User searcher, IQuery query)
        {
            var helper = CreateIndexedInputHelper();
            return helper.GetAllNavigatorValuesForMatchingClauses(searcher, SystemSearchConstants.ForCatalog().JqlClauseNames, query);
        }

        public virtual IClause GetSearchClause(User user, IFieldValuesHolder fieldValuesHolder)
        {
            var projects = (IList<string>)fieldValuesHolder[SystemSearchConstants.ForCatalog().UrlParameter];
            if (projects != null && projects.Count > 0)
            {
                var operands = new List<IOperand>();
                foreach (string idStr in projects)
                {
                    // remove the "ALL" flag
                    if (!idStr.Equals("-1"))
                    {
                        operands.Add(GetProjectOperandForIdString(idStr));
                    }
                }
                if (operands.Count == 1)
                {
                    return new TerminalClause(SystemSearchConstants.ForCatalog().JqlClauseNames.PrimaryName, Operator.EQUALS, operands[0]);
                }
                if (operands.Count > 1)
                {
                    return new TerminalClause(SystemSearchConstants.ForCatalog().JqlClauseNames.PrimaryName, Operator.IN, new MultiValueOperand(operands));
                }
            }
            return null;
        }

        /// <summary>
        /// Attempts to resolve the input string as an id for a project.
        /// </summary>
        /// <param name="idStr"> the id string </param>
        /// <returns> an operand that is the project's key. If the project does not exist, the id is returned as a long.
        /// If the id was non-numeric the id is returned as a string. </returns>
        private SingleValueOperand GetProjectOperandForIdString(string idStr)
        {
            try
            {
                var id = Convert.ToInt32(idStr);
                var catalog = _catalogService.GetCatalog(id);
                var o = catalog == null ? new SingleValueOperand(id) : new SingleValueOperand(catalog.Id);
                return o;
            }
            catch (Exception e)
            {
                // we got some invalid catalog id - we will fall back to using String as the operand
                return new SingleValueOperand(idStr);
            }
        }

        /// <summary>
        /// Sets the project Id in session if only one project was selected.
        /// </summary>
        protected virtual ISet<string> CatalogIdInSession
        {
            set
            {
                if (value != null && value.Count() == 1)
                {
                    var iter = value.GetEnumerator();
                    iter.MoveNext();

                    string idStr = iter.Current;
                    var id = Convert.ToInt32(idStr);

                    Catalog project = _catalogService.GetCatalog(id);
                    if (project != null)
                    {
                        //projectHistoryManager.addProjectToHistory(authenticationContext.LoggedInUser, project);
                    }
                }
            }
        }

        protected virtual IIndexedInputHelper CreateIndexedInputHelper()
        {
            return new IndexedInputHelper<Catalog>(_projectIndexInfoResolver, _operandResolver, _fieldFlagOperandRegistry);
        }
    }
}