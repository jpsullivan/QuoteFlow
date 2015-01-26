using System;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;
using QuoteFlow.Core.Util;

namespace QuoteFlow.Core.Jql.Validator
{
    /// <summary>
    /// A clause validator that can be used for multiple constant (priority, status, resolution) clause types.
    /// </summary>
    public abstract class ValuesExistValidator
    {
        private readonly IJqlOperandResolver _operandResolver;
        private readonly MessageSetLevel _level;

        internal ValuesExistValidator(IJqlOperandResolver operandResolver, MessageSetLevel level = MessageSetLevel.Error)
        {
            if (operandResolver == null)
            {
                throw new ArgumentNullException("operandResolver");
            }

            _operandResolver = operandResolver;
            _level = level;
        }

        internal virtual IMessageSet Validate(User searcher, ITerminalClause terminalClause)
        {
            var operand = terminalClause.Operand;
            string fieldName = terminalClause.Name;

            var messages = new ListOrderedMessageSet();

            if (_operandResolver.IsValidOperand(operand))
            {
                // visit every query literal and determine lookup failures
                var rawValues = _operandResolver.GetValues(searcher, operand, terminalClause);
                foreach (QueryLiteral rawValue in rawValues)
                {
                    if (rawValue.StringValue != null)
                    {
                        if (!StringValueExists(searcher, rawValue.StringValue))
                        {
                            if (_operandResolver.IsFunctionOperand(rawValue.SourceOperand))
                            {
                                messages.AddMessage(_level, string.Format("jira.jql.clause.no.value.for.name.from.function: {0}, {1}", rawValue.SourceOperand.Name, fieldName));
                            }
                            else
                            {
                                messages.AddMessage(_level, string.Format("jira.jql.clause.no.value.for.name: {0}, {1}", fieldName, rawValue.StringValue));
                            }
                        }
                    }
                    else if (rawValue.IntValue != null)
                    {
                        if (!IntValueExists(searcher, rawValue.IntValue))
                        {
                            if (_operandResolver.IsFunctionOperand(rawValue.SourceOperand))
                            {
                                messages.AddMessage(_level, string.Format("jira.jql.clause.no.value.for.name.from.function: {0}, {1}", rawValue.SourceOperand.Name, fieldName));
                            }
                            else
                            {
                                messages.AddMessage(_level, string.Format("jira.jql.clause.no.value.for.id: {0}, {1}", fieldName, rawValue.IntValue.ToString()));
                            }
                        }
                    }
                }
            }

            return messages;
        }

        internal abstract bool StringValueExists(User searcher, string value);

        internal abstract bool IntValueExists(User searcher, int? value);
    }
}