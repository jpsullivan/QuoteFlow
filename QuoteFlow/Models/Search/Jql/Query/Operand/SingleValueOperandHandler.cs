using System.Collections.Generic;
using QuoteFlow.Infrastructure.Util;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Query.Clause;

namespace QuoteFlow.Models.Search.Jql.Query.Operand
{
    public class SingleValueOperandHandler : IOperandHandler<SingleValueOperand>
    {
        public virtual IMessageSet Validate(User searcher, SingleValueOperand operand, ITerminalClause terminalClause)
        {
            return new MessageSet();
        }

        public virtual IEnumerable<QueryLiteral> GetValues(IQueryCreationContext queryCreationContext, SingleValueOperand operand, ITerminalClause terminalClause)
        {
            var value = operand.IntValue == null
                ? new QueryLiteral(operand, operand.StringValue)
                : new QueryLiteral(operand, operand.IntValue);
            return new List<QueryLiteral> { value };
        }

        public bool IsList()
        {
            return false;
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