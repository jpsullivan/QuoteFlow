using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;
using QuoteFlow.Models.Assets.Search.Constants;
using QuoteFlow.Models.Assets.Transport;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Query.Operand;
using QuoteFlow.Models.Search.Jql.Resolver;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Models.Assets.Search.Searchers.Transformer
{
    /// <summary>
    /// The SearchInputTransformer for the Catalog system field.
    /// </summary>
    public class ProjectSearchInputTransformer : ISearchInputTransformer
    {
        private readonly FieldFlagOperandRegistry fieldFlagOperandRegistry;
        private readonly IJqlOperandResolver operandResolver;
        private readonly CatalogIndexInfoResolver projectIndexInfoResolver;
        private readonly ICatalogService catalogService;

        public ProjectSearchInputTransformer(CatalogIndexInfoResolver projectIndexInfoResolver, IJqlOperandResolver operandResolver, FieldFlagOperandRegistry fieldFlagOperandRegistry, ICatalogService catalogService)
        {
            this.fieldFlagOperandRegistry = fieldFlagOperandRegistry;
            this.operandResolver = operandResolver;
            this.projectIndexInfoResolver = projectIndexInfoResolver;
            this.catalogService = catalogService;
        }

        public virtual void PopulateFromParams(User user, IFieldValuesHolder fieldValuesHolder, IActionParams actionParams)
        {
            string url = SystemSearchConstants.ForCatalog().UrlParameter;
            var @params = actionParams.GetValuesForKey(url).ToList();
            fieldValuesHolder.Add(url, @params == null ? null : new HashSet<string>());
        }

        public void ValidateParams(User searcher, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder, ModelState errors)
        {
            // We currently dont do anything
        }

        public virtual void PopulateFromQuery(User user, IFieldValuesHolder fieldValuesHolder, IQuery query, ISearchContext searchContext)
        {
            var uncleanedValues = GetNavigatorValuesAsStrings(user, query);

            IList<string> values = new List<string>(uncleanedValues);
            CollectionUtils.transform(values, JiraTransformers.NULL_SWAP);
            fieldValuesHolder.put(SystemSearchConstants.forProject().UrlParameter, values);
            CatalogIdInSession = uncleanedValues;
        }

        public virtual bool DoRelevantClausesFitFilterForm(User user, IQuery query, ISearchContext searchContext)
        {
            return CreateNavigatorStructureChecker().checkSearchRequest(query);
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
            var projects = (IList<string>)fieldValuesHolder.get(SystemSearchConstants.ForCatalog().UrlParameter);
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
                var catalog = catalogService.GetCatalog(id);
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
        /// <param name="selectedProjectIds"> Collecyion of Longs. Assumes that all non-null Long are valid project Ids </param>
        internal virtual ISet<string> CatalogIdInSession
        {
            set
            {
                if (value != null && value.Count() == 1)
                {
                    var iter = value.GetEnumerator();
                    iter.MoveNext();

                    string idStr = iter.Current;
                    var id = Convert.ToInt32(idStr);

                    Catalog project = catalogService.GetCatalog(id);
                    if (project != null)
                    {
                        //projectHistoryManager.addProjectToHistory(authenticationContext.LoggedInUser, project);
                    }
                }
            }
        }
        
        internal virtual IndexedInputHelper CreateIndexedInputHelper()
        {
            return new DefaultIndexedInputHelper<Catalog>(projectIndexInfoResolver, operandResolver, fieldFlagOperandRegistry);
        }
    }

}