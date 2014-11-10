using System.Collections.Generic;
using QuoteFlow.Infrastructure.Util;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Query.Clause;

namespace QuoteFlow.Models.Search.Jql.Query.Operand
{
    public class MultiValueOperandHandler : IOperandHandler<MultiValueOperand>
    {
        private readonly IJqlOperandResolver operandResolver;

        public MultiValueOperandHandler(IJqlOperandResolver operandResolver)
        {
            this.operandResolver = operandResolver;
        }

        public virtual IMessageSet Validate(User searcher, MultiValueOperand operand, ITerminalClause terminalClause)
        {
            var messages = new MessageSet();
            foreach (var subOperand in operand.Values)
            {
                IMessageSet subMessageSet = operandResolver.Validate(searcher, subOperand, terminalClause);
                if (subMessageSet.HasAnyErrors())
                {
                    messages.AddMessageSet(subMessageSet);
                }
            }
            return messages;
        }

        public virtual IEnumerable<QueryLiteral> GetValues(IQueryCreationContext queryCreationContext, MultiValueOperand operand, ITerminalClause terminalClause)
        {
            var valuesList = new List<QueryLiteral>();
            foreach (var subOperand in operand.Values)
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