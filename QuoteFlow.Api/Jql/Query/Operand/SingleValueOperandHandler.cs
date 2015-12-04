using System.Collections.Generic;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Api.Jql.Query.Operand
{
    public class SingleValueOperandHandler : IOperandHandler<SingleValueOperand>
    {
        public IMessageSet Validate(User searcher, SingleValueOperand operand, ITerminalClause terminalClause)
        {
            // no validation needed
            return new MessageSet();
        }

        public IEnumerable<QueryLiteral> GetValues(IQueryCreationContext queryCreationContext, SingleValueOperand operand, ITerminalClause terminalClause)
        {
            var svo = operand;
            QueryLiteral value;
            if (svo.IntValue != null)
            {
                value = new QueryLiteral(svo, svo.IntValue);
                
            }
            else if (svo.DecimalValue != null)
            {
                value = new QueryLiteral(svo, svo.DecimalValue);
            }
            else
            {
                value = new QueryLiteral(svo, svo.StringValue);
            }

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