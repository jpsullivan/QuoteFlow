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
    public class FunctionOperandHandler : IOperandHandler<FunctionOperand>
	{
		public FunctionOperandHandler(IJqlFunction jqlFunction)
		{
		    if (jqlFunction == null)
		    {
		        throw new ArgumentNullException(nameof(jqlFunction));
		    }

			JqlFunction = jqlFunction;
		}

        public IMessageSet Validate(User searcher, FunctionOperand operand, ITerminalClause terminalClause)
        {
            return JqlFunction.Validate(searcher, operand, terminalClause);
        }

        public IEnumerable<QueryLiteral> GetValues(IQueryCreationContext queryCreationContext, FunctionOperand operand, ITerminalClause terminalClause)
        {
            return JqlFunction.GetValues(queryCreationContext, operand, terminalClause);
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