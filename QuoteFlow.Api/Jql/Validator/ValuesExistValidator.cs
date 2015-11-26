using System;
using System.Collections.Generic;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Api.Jql.Validator
{
    /// <summary>
    /// A clause validator that can be used for multiple constant clause types.
    /// </summary>
    public abstract class ValuesExistValidator
    {
        private readonly IJqlOperandResolver _operandResolver;
        private readonly MessageSetLevel _level;

        internal ValuesExistValidator(IJqlOperandResolver operandResolver, MessageSetLevel level = MessageSetLevel.Error)
        {
            if (operandResolver == null)
            {
                throw new ArgumentNullException(nameof(operandResolver));
            }

            _operandResolver = operandResolver;
            _level = level;
        }

        internal IMessageSet Validate(TerminalClause terminalClause, QueryCreationContext queryCreationContext)
        {
            var operand = terminalClause.Operand;
            var rawOperandValues = _operandResolver.GetValues(queryCreationContext, operand, terminalClause);

            return Validate(queryCreationContext.User, terminalClause, rawOperandValues);
        }

        internal IMessageSet Validate(User searcher, ITerminalClause terminalClause)
        {
            var operand = terminalClause.Operand;
            var rawOperandValues = _operandResolver.GetValues(searcher, operand, terminalClause);

            return Validate(searcher, terminalClause, rawOperandValues);
        }

        private IMessageSet Validate(User searcher, ITerminalClause terminalClause, IEnumerable<QueryLiteral> rawOperandValues)
        {
            var operand = terminalClause.Operand;
            string fieldName = terminalClause.Name;

            IMessageSet messages = new ListOrderedMessageSet();

            if (_operandResolver.IsValidOperand(operand))
            {
                // visit every query literal and determine lookup failures
                foreach (QueryLiteral rawValue in rawOperandValues)
                {
                    if (rawValue.StringValue != null)
                    {
                        if (!StringValueExists(searcher, rawValue.StringValue))
                        {
                            if (_operandResolver.IsFunctionOperand(rawValue.SourceOperand))
                            {
                                messages.AddMessage(_level,
                                    $"quoteflow.jql.clause.no.value.for.name.from.function, operand name:{rawValue.SourceOperand.Name}, field name:{fieldName}");
                            }
                            else
                            {
                                messages.AddMessage(_level,
                                    $"quoteflow.jql.clause.no.value.for.name, field name:{fieldName}, operand name:{rawValue.StringValue}");
                            }
                        }
                    }
                    else if (rawValue.IntValue != null)
                    {
                        if (!IntValueExist(searcher, rawValue.IntValue))
                        {
                            if (_operandResolver.IsFunctionOperand(rawValue.SourceOperand))
                            {
                                messages.AddMessage(_level,
                                    $"quoteflow.jql.clause.no.value.for.name.from.function, operand name:{rawValue.SourceOperand.Name}, field name:{fieldName}");
                            }
                            else
                            {
                                messages.AddMessage(_level,
                                    $"quoteflow.jql.clause.no.value.for.id, field name:{fieldName}, rawValue:{rawValue.IntValue}");
                            }
                        }
                    }
                }
            }

            return messages;
        }

        protected abstract bool StringValueExists(User searcher, string value);

        protected abstract bool IntValueExist(User searcher, int? value);

    }
}