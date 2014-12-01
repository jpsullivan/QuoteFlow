using QuoteFlow.Infrastructure;
using QuoteFlow.Infrastructure.Util;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Query.Operand;

namespace QuoteFlow.Models.Search.Jql.Function
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