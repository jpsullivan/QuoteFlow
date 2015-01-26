using System.Collections.Generic;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Api.Jql.Query.Operand
{
    public class SingleValueOperandHandler : IOperandHandler<IOperand>
    {
        public IMessageSet Validate(User searcher, IOperand operand, ITerminalClause terminalClause)
        {
            // no validation needed
            return new MessageSet();
        }

        public IEnumerable<QueryLiteral> GetValues(IQueryCreationContext queryCreationContext, IOperand operand, ITerminalClause terminalClause)
        {
            var svo = (SingleValueOperand) operand;
            var value = svo.IntValue == null
                ? new QueryLiteral(svo, svo.StringValue)
                : new QueryLiteral(svo, svo.IntValue);
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