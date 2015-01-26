using System.Collections.Generic;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Api.Jql.Query.Operand
{
    public class MultiValueOperandHandler : IOperandHandler<IOperand>
    {
        private readonly IJqlOperandResolver operandResolver;

        public MultiValueOperandHandler(IJqlOperandResolver operandResolver)
        {
            this.operandResolver = operandResolver;
        }

        public IMessageSet Validate(User searcher, IOperand operand, ITerminalClause terminalClause)
        {
            var mvo = (MultiValueOperand) operand;
            var messages = new MessageSet();
            foreach (var subOperand in mvo.Values)
            {
                IMessageSet subMessageSet = operandResolver.Validate(searcher, subOperand, terminalClause);
                if (subMessageSet.HasAnyErrors())
                {
                    messages.AddMessageSet(subMessageSet);
                }
            }
            return messages;
        }

        public IEnumerable<QueryLiteral> GetValues(IQueryCreationContext queryCreationContext, IOperand operand, ITerminalClause terminalClause)
        {
            var mvo = (MultiValueOperand) operand;
            var valuesList = new List<QueryLiteral>();
            foreach (var subOperand in mvo.Values)
            {
                var vals = operandResolver.GetValues(queryCreationContext, subOperand, terminalClause);
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