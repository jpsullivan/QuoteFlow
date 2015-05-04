using System;
using System.Collections.Generic;
using QuoteFlow.Api.Jql.Function;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Api.Jql.Query.Operand
{
    /// <summary>
    /// Adapter to convert the plugin point <see cref="JqlFunction"/> into
    /// <see cref="OperandHandler"/>.
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