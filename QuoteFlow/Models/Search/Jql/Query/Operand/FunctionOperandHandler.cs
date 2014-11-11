using System;
using System.Collections.Generic;
using QuoteFlow.Infrastructure.Util;
using QuoteFlow.Models.Search.Jql.Function;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Query.Clause;

namespace QuoteFlow.Models.Search.Jql.Query.Operand
{
    /// <summary>
    /// Adapter to convert the plugin point <seealso cref="JqlFunction"/> into
    /// <seealso cref="OperandHandler"/>.
    /// </summary>
    public class FunctionOperandHandler : IOperandHandler<IOperand>
	{
		public FunctionOperandHandler(IJqlFunction jqlFunction)
		{
		    if (jqlFunction == null)
		    {
		        throw new ArgumentNullException("jqlFunction");
		    }

			JqlFunction = jqlFunction;
		}

        public IMessageSet Validate(User searcher, IOperand operand, ITerminalClause terminalClause)
        {
            return JqlFunction.Validate(searcher, (FunctionOperand) operand, terminalClause);
        }

        public IEnumerable<QueryLiteral> GetValues(IQueryCreationContext queryCreationContext, IOperand operand, ITerminalClause terminalClause)
        {
            return JqlFunction.GetValues(queryCreationContext, (FunctionOperand) operand, terminalClause);
        }

        public bool IsList()
        {
            return JqlFunction.IsList();
        }

        public bool IsEmpty()
        {
            return false;
        }

        public bool IsFunction()
        {
            return true;
        }

        public IJqlFunction JqlFunction { get; set; }
	}
}