using System.Collections.Generic;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Api.Jql.Query.Operand
{
    /// <summary>
    /// Handles the <see cref="EmptyOperand"/>.
    /// </summary>
    public class EmptyOperandHandler : IOperandHandler<IOperand>
    {
        /// <summary>
        /// Returns a single empty query literal.
        /// </summary>
        /// <param name="queryCreationContext"></param>
        /// <param name="operand"></param>
        /// <param name="terminalClause"></param>
        /// <returns></returns>
        public virtual IEnumerable<QueryLiteral> GetValues(IQueryCreationContext queryCreationContext, IOperand operand, ITerminalClause terminalClause)
        {
            return new List<QueryLiteral> { new QueryLiteral(operand) };
        }

        public IMessageSet Validate(User searcher, IOperand operand, ITerminalClause terminalClause)
        {
            // We don't need to do any validation
            return new MessageSet();
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