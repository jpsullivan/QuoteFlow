using System.Collections.Generic;
using QuoteFlow.Infrastructure.Util;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Query.Clause;

namespace QuoteFlow.Models.Search.Jql.Query.Operand
{
    /// <summary>
    /// Handles the <see cref="EmptyOperand"/>.
    /// </summary>
    public class EmptyOperandHandler : IOperandHandler<EmptyOperand>
    {
        public virtual IMessageSet Validate(User searcher, EmptyOperand operand, ITerminalClause terminalClause)
        {
            // We don't need to do any validation
            return new MessageSet();
        }

        /*
         * Returns a single empty query literal.
         */
        public virtual IEnumerable<QueryLiteral> GetValues(IQueryCreationContext queryCreationContext, EmptyOperand operand, ITerminalClause terminalClause)
        {
            return new List<QueryLiteral> {new QueryLiteral(operand)};
        }

        public bool IsList()
        {
            return false;
        }

        public bool IsEmpty()
        {
            return true;
        }

        public bool IsFunction()
        {
            return false;
        }
    }

}