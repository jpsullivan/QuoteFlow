using System;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Core.Jql.Validator
{
    /// <summary>
    /// Validates decimal values.
    /// </summary>
    public class DecimalValueValidator
    {
        private readonly IJqlOperandResolver _operandResolver;

        public DecimalValueValidator(IJqlOperandResolver operandResolver)
        {
            if (operandResolver == null) throw new ArgumentNullException(nameof(operandResolver));
            _operandResolver = operandResolver;
        }

        public IMessageSet Validate(User searcher, ITerminalClause terminalClause)
        {
            if (terminalClause == null) throw new ArgumentNullException(nameof(terminalClause));

            var operand = terminalClause.Operand;
            var messages = new MessageSet();

            if (_operandResolver.IsValidOperand(operand))
            {
                var values = _operandResolver.GetValues(searcher, operand, terminalClause);
                var fieldName = terminalClause.Name;

                foreach (var queryLiteral in values)
                {
                    bool isValid = true;

                    var str = queryLiteral.StringValue;
                    if (str != null)
                    {
                        try
                        {
                            var d = Convert.ToDecimal(str);
                        }
                        catch (FormatException)
                        {
                            isValid = false;
                        }
                    }

                    if (isValid)
                    {
                        continue;
                    }

                    string msg;
                    if (_operandResolver.IsFunctionOperand(queryLiteral.SourceOperand))
                    {
                        msg = $"quoteflow.jql.clause.integer.format.invalid.from.func: {queryLiteral.SourceOperand.Name}";
                    }
                    else
                    {
                        msg = $"quoteflow.jql.clause.integer.format.invalid: {str}, {fieldName}";
                    }
                    messages.AddErrorMessage(msg);
                }
            }

            return messages;
        }
    }
}