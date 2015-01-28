using System;
using Lucene.Net.QueryParsers;
using Ninject;
using QuoteFlow.Api.Asset.Search.Searchers.Transformer;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Operator;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Jql.Validator;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;
using QuoteFlow.Core.DependencyResolution;
using QuoteFlow.Core.Jql.Query.Lucene.Parsing;

namespace QuoteFlow.Core.Jql.Validator
{
    /// <summary>
    /// A generic validator for text fields
    /// </summary>
    public class FreeTextFieldValidator : IClauseValidator
    {
        private readonly string indexField;
        private readonly IJqlOperandResolver operandResolver;
        private readonly TextQueryValidator textQueryValidator;

        public FreeTextFieldValidator(string indexField, IJqlOperandResolver operandResolver)
        {
            if (string.IsNullOrEmpty(indexField))
            {
                throw new ArgumentException("IndexField cannot be empty.", "indexField");
            }

            if (operandResolver == null)
            {
                throw new ArgumentNullException("operandResolver");
            }

            this.indexField = indexField;
            this.operandResolver = operandResolver;
            this.textQueryValidator = new TextQueryValidator();
        }

        public virtual IMessageSet Validate(User searcher, ITerminalClause terminalClause)
        {
            var messageSet = new MessageSet();
            Operator @operator = terminalClause.Operator;
            string fieldName = terminalClause.Name;
            if (!HandlesOperator(@operator))
            {
                messageSet.AddErrorMessage(string.Format("jira.jql.clause.does.not.support.operator: {0}, {1}, {2}", @operator.GetDisplayAttributeFrom(typeof(Operator)), fieldName));
                return messageSet;
            }

            IOperand operand = terminalClause.Operand;

            var values = operandResolver.GetValues(searcher, operand, terminalClause);
            if (values != null)
            {
                foreach (QueryLiteral literal in values)
                {
                    // empty literals are always okay
                    if (!literal.IsEmpty)
                    {
                        string query = literal.AsString();
                        if (query.HasValue())
                        {
                            string functionName = operandResolver.IsFunctionOperand(literal.SourceOperand) ? literal.SourceOperand.Name : null;

                            var validationResult = textQueryValidator.Validate(GetQueryParser(indexField), query, fieldName, functionName, false);
                            messageSet.AddMessageSet(validationResult);
                        }
                        else
                        {
                            messageSet.AddErrorMessage(string.Format("jira.jql.text.clause.does.not.support.empty: {0}", fieldName));
                        }
                    }
                }
            }
            else
            {
                //                // This should never be allowed to happen since we do not allow list operands with '~' so lets log it
                //                log.error("Text field validation was provided an operand handler that gave us back more than one value when validating '" + fieldName + "'.");
            }

            return messageSet;
        }

        private static bool HandlesOperator(Operator @operator)
        {
            return OperatorClasses.TextOperators.Contains(@operator);
        }

        internal virtual QueryParser GetQueryParser(string indexField)
        {
            return Container.Kernel.TryGet<LuceneQueryParserFactory>().CreateParserFor(indexField);
            //return ComponentAccessor.getComponent(typeof(LuceneQueryParserFactory)).createParserFor(indexField);
        }
    }
}