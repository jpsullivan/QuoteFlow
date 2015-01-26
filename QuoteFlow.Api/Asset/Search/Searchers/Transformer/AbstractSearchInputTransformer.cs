using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Lucene.Net.QueryParsers;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Transport;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Api.Asset.Search.Searchers.Transformer
{
    /// <summary>
    /// Common capabilities for SearchInputTransformer implementations.
    /// </summary>
    public abstract class AbstractSearchInputTransformer : ISearchInputTransformer
    {
        protected internal IJqlOperandResolver operandResolver;
        protected internal readonly string fieldsKey;
        protected internal readonly string id;
        private readonly TextQueryValidator textQueryValidator;

        public AbstractSearchInputTransformer(IJqlOperandResolver operandResolver, string id, string fieldsKey)
        {
            if (operandResolver == null)
            {
                throw new ArgumentNullException("operandResolver");
            }

            this.operandResolver = operandResolver;
            this.id = id;
            this.fieldsKey = fieldsKey;
            this.textQueryValidator = new TextQueryValidator();
        }

        protected internal bool HasDuplicates(IEnumerable<ITerminalClause> foundChildren)
        {
            ISet<string> containsSet = new HashSet<string>();
            foreach (TerminalClause foundChild in foundChildren)
            {
                if (!containsSet.Add(foundChild.Name))
                {
                    return true;
                }
            }
            return false;
        }

        protected internal bool HasEmpties(IEnumerable<ITerminalClause> foundChildren)
        {
            foreach (TerminalClause foundChild in foundChildren)
            {
                var operand = foundChild.Operand;
                if (operandResolver.IsEmptyOperand(operand))
                {
                    return true;
                }
            }
            return false;
        }

        protected internal string GetValueForField(IEnumerable<ITerminalClause> terminalClauses, User user, params string[] jqlClauseNames)
        {
            return GetValueForField(terminalClauses, user, jqlClauseNames.ToList());
        }

        protected internal string GetValueForField(IEnumerable<ITerminalClause> terminalClauses, User user, ICollection<string> jqlClauseNames)
        {
            TerminalClause theClause = null;
            foreach (TerminalClause terminalClause in terminalClauses)
            {
                if (jqlClauseNames.Contains(terminalClause.Name))
                {
                    // if there was already a clause with the same name, then return null
                    if (theClause != null)
                    {
                        return null;
                    }
                    theClause = terminalClause;
                }
            }

            if (theClause != null)
            {
                var operand = theClause.Operand;
                QueryLiteral rawValue = operandResolver.GetSingleValue(user, operand, theClause);
                if (rawValue != null && !rawValue.IsEmpty)
                {
                    return rawValue.AsString();
                }
            }
            return null;
        }

        public void ValidateParams(User user, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder)
        {
            string query = (string)fieldValuesHolder[id];

            if (query.HasValue())
            {

                IMessageSet validationResult = textQueryValidator.Validate(CreateQueryParser(), query, id, null, true);
                foreach (String errorMessage in validationResult.ErrorMessages)
                {
                    //errors.AddError(id, errorMessage);
                }
            }
        }

        internal QueryParser CreateQueryParser()
        {
            // We pass in the summary index field here, because we dont actually care about the lhs of the query, only that
            // user input can be parsed.
            return Container.Kernel.TryGet<LuceneQueryParserFactory>().CreateParserFor(SystemSearchConstants.ForSummary().IndexField);
        }

        public abstract void PopulateFromParams(User searcher, IFieldValuesHolder fieldValuesHolder, IActionParams actionParams);
        public abstract void ValidateParams(User searcher, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder, ModelState errors);
        public abstract void PopulateFromQuery(User searcher, IFieldValuesHolder fieldValuesHolder, IQuery query, ISearchContext searchContext);
        public abstract bool DoRelevantClausesFitFilterForm(User searcher, IQuery query, ISearchContext searchContext);
        public abstract IClause GetSearchClause(User searcher, IFieldValuesHolder fieldValuesHolder);
    }
}