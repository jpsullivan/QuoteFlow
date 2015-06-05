using System.Collections.Generic;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Jql.Query.Operand.Registry;

namespace QuoteFlow.Core.Tests.Jql.Validator
{
    /// <summary>
    /// Simple mock interface for the OperandRegistry that uses a dictionary 
    /// to serve the handlers.
    /// </summary>
    public class MockJqlFunctionHandlerRegistry : IJqlFunctionHandlerRegistry
    {
        private readonly IDictionary<string, FunctionOperandHandler> _handlers;

        public MockJqlFunctionHandlerRegistry()
            : this(new Dictionary<string, FunctionOperandHandler>())
        {
        }

        public MockJqlFunctionHandlerRegistry(IDictionary<string, FunctionOperandHandler> handlers)
        {
            _handlers = handlers;
        }

        public FunctionOperandHandler GetOperandHandler(FunctionOperand operand)
        {
            return _handlers[operand.Name];
        }

        public IEnumerable<string> AllFunctionNames { get; private set; }
    }
}