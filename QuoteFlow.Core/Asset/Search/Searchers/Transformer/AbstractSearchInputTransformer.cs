using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Lucene.Net.QueryParsers;
using Ninject;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Search.Searchers.Transformer;
using QuoteFlow.Api.Asset.Transport;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Lucene.Parsing;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;
using QuoteFlow.Core.DependencyResolution;

namespace QuoteFlow.Core.Asset.Search.Searchers.Transformer
{
    /// <summary>
    /// Common capabilities for SearchInputTransformer implementations.
    /// </summary>
    public abstract class AbstractSearchInputTransformer : ISearchInputTransformer
    {
        protected readonly IJqlOperandResolver OperandResolver;
        protected readonly string FieldsKey;
        protected readonly string Id;

        private readonly TextQueryValidator _textQueryValidator;

        public AbstractSearchInputTransformer(IJqlOperandResolver operandResolver, string id, string fieldsKey)
        {
            if (operandResolver == null)
            {
                throw new ArgumentNullException(nameof(operandResolver));
            }

            OperandResolver = operandResolver;
            Id = id;
            FieldsKey = fieldsKey;
            _textQueryValidator = new TextQueryValidator();
        }

        protected internal bool HasDuplicates(IEnumerable<ITerminalClause> foundChildren)
        {
            ISet<string> containsSet = new HashSet<string>();
            return foundChildren.Any(foundChild => !containsSet.Add(foundChild.Name));
        }

        protected internal bool HasEmpties(IEnumerable<ITerminalClause> foundChildren)
        {
            return foundChildren.Select(foundChild => foundChild.Operand).Any(operand => OperandResolver.IsEmptyOperand(operand));
        }

        protected internal string GetValueForField(IEnumerable<ITerminalClause> terminalClauses, User user, params string[] jqlClauseNames)
        {
            return GetValueForField(terminalClauses, user, jqlClauseNames.ToList());
        }

        private string GetValueForField(IEnumerable<ITerminalClause> terminalClauses, User user, ICollection<string> jqlClauseNames)
        {
            ITerminalClause theClause = null;
            foreach (var terminalClause in terminalClauses)
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
                QueryLiteral rawValue = OperandResolver.GetSingleValue(user, operand, theClause);
                if (rawValue != null && !rawValue.IsEmpty)
                {
                    return rawValue.AsString();
                }
            }
            return null;
        }

        public void ValidateParams(User user, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder)
        {
            string query = (string)fieldValuesHolder[Id];

            if (query.HasValue())
            {

                IMessageSet validationResult = _textQueryValidator.Validate(CreateQueryParser(), query, Id, null, true);
                foreach (String errorMessage in validationResult.ErrorMessages)
                {
                    //errors.AddError(id, errorMessage);
                }
            }
        }

        private static QueryParser CreateQueryParser()
        {
            // We pass in the summary index field here, because we dont actually care about the lhs of the query, only that
            // user input can be parsed.
            return Container.Kernel.TryGet<ILuceneQueryParserFactory>().CreateParserFor(SystemSearchConstants.ForSummary().IndexField);
        }

        public abstract void PopulateFromParams(User searcher, IFieldValuesHolder fieldValuesHolder, IActionParams actionParams);
        public abstract void ValidateParams(User searcher, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder, ModelState errors);
        public abstract void PopulateFromQuery(User searcher, IFieldValuesHolder fieldValuesHolder, IQuery query, ISearchContext searchContext);
        public abstract bool DoRelevantClausesFitFilterForm(User searcher, IQuery query, ISearchContext searchContext);
        public abstract IClause GetSearchClause(User searcher, IFieldValuesHolder fieldValuesHolder);
    }
}