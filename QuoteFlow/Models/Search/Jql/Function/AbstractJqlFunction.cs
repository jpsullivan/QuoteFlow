using System;
using System.Collections.Generic;
using QuoteFlow.Infrastructure;
using QuoteFlow.Infrastructure.Util;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Query.Operand;
using QuoteFlow.Models.Search.Jql.Validator;

namespace QuoteFlow.Models.Search.Jql.Function
{
    /// <summary>
    /// A useful base implementation of the <seealso cref="IJqlFunction"/> interface, that
    /// provides sensible default behaviour for the <seealso cref="#init(JqlFunctionModuleDescriptor)"/>, <seealso cref="GetFunctionName()"/>
    /// and <seealso cref="IsList()"/> methods. You should not need to override these methods in your implementation.
    /// </summary>
    public abstract class AbstractJqlFunction : IJqlFunction
    {
        protected IMessageSet ValidateNumberOfArgs(FunctionOperand operand, int expected)
        {
            return new NumberOfArgumentsValidator(expected).Validate(operand);
        }

        public IMessageSet Validate(User searcher, FunctionOperand operand, ITerminalClause terminalClause)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<QueryLiteral> GetValues(IQueryCreationContext queryCreationContext, FunctionOperand operand, ITerminalClause terminalClause)
        {
            throw new NotImplementedException();
        }

        public bool IsList()
        {
            return false;
        }

        public int MinimumNumberOfExpectedArguments { get; private set; }
        public string FunctionName { get; private set; }
        public IQuoteFlowDataType DataType { get; private set; }
    }
}