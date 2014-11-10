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
    public class FunctionOperandHandler : IOperandHandler<FunctionOperand>
	{
		public FunctionOperandHandler(IJqlFunction jqlFunction)
		{
		    if (jqlFunction == null)
		    {
		        throw new ArgumentNullException("jqlFunction");
		    }

			JqlFunction = jqlFunction;
		}

		public virtual IMessageSet Validate(User searcher, FunctionOperand operand, ITerminalClause terminalClause)
		{
		    return JqlFunction.Validate(searcher, operand, terminalClause);
		}

        public IEnumerable<QueryLiteral> GetValues(IQueryCreationContext queryCreationContext, FunctionOperand operand,
            ITerminalClause terminalClause)
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