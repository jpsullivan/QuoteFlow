using System.Collections.Generic;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Api.Jql.Query.Operand
{
    public class MultiValueOperandHandler : IOperandHandler<MultiValueOperand>
    {
        private readonly IJqlOperandResolver _operandResolver;

        public MultiValueOperandHandler(IJqlOperandResolver operandResolver)
        {
            _operandResolver = operandResolver;
        }

        public IMessageSet Validate(User searcher, MultiValueOperand operand, ITerminalClause terminalClause)
        {
            var messages = new MessageSet();
            foreach (var subOperand in operand.Values)
            {
                IMessageSet subMessageSet = _operandResolver.Validate(searcher, subOperand, terminalClause);
                if (subMessageSet.HasAnyErrors())
                {
                    messages.AddMessageSet(subMessageSet);
                }
            }
            return messages;
        }

        public IEnumerable<QueryLiteral> GetValues(IQueryCreationContext queryCreationContext, MultiValueOperand operand, ITerminalClause terminalClause)
        {
            var valuesList = new List<QueryLiteral>();
            foreach (var subOperand in operand.Values)
            {
                var vals = _operandResolver.GetValues(queryCreationContext, subOperand, terminalClause);
                if (vals != null)
                {
                    valuesList.AddRange(vals);
                }
            }
            return valuesList;
        }

        public bool IsList()
        {
            return true;
        }

        public bool IsEmpty()
        {
            return false;
        }

        public bool IsFunction()
        {
            return false;
        }
    }

}