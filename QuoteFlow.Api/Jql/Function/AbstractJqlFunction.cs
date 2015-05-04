using System;
using System.Collections.Generic;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Jql.Validator;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Api.Jql.Function
{
    /// <summary>
    /// A useful base implementation of the <see cref="IJqlFunction"/> interface, that
    /// provides sensible default behaviour for the <see cref="#init(JqlFunctionModuleDescriptor)"/>, <see cref="GetFunctionName()"/>
    /// and <see cref="IsList()"/> methods. You should not need to override these methods in your implementation.
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