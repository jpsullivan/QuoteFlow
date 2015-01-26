using QuoteFlow.Api;
using QuoteFlow.Api.Jql.Function;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Core.Jql.Function
{
    /// <summary>
    /// Base class for manufacturer functions.
    /// </summary>
    public abstract class AbstractManufacturerFunction : AbstractJqlFunction
    {
        protected internal AbstractManufacturerFunction()
		{
		}

		public virtual IMessageSet Validate(User searcher, FunctionOperand operand, ITerminalClause terminalClause)
		{
			return ValidateNumberOfArgs(operand, 0);
		}

		public virtual int MinimumNumberOfExpectedArguments
		{
			get { return 0; }
		}

		public virtual IQuoteFlowDataType DataType
		{
			get { return QuoteFlowDataTypes.Manufacturer; }
		}
    }
}