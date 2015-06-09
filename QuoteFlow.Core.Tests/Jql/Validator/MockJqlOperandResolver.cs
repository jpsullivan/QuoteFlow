using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Core.Tests.Jql.Validator
{
    /// <summary>
    /// Simple mock of <see cref="IJqlOperandResolver"/> for testing.
    /// </summary>
    public class MockJqlOperandResolver : IJqlOperandResolver
    {
        private readonly IDictionary<string, IOperandHandler<IOperand>> _handlers;

        public MockJqlOperandResolver() : this(new Dictionary<string, IOperandHandler<IOperand>>())
		{
		}

		public MockJqlOperandResolver(IDictionary<string, IOperandHandler<IOperand>> handlers)
		{
			_handlers = handlers;
		}

        public virtual MockJqlOperandResolver AddHandlers(IDictionary<string, IOperandHandler<IOperand>> handlers)
		{
            foreach (var operandHandler in handlers)
            {
                _handlers.Add(operandHandler);
            }

			return this;
		}

		public virtual MockJqlOperandResolver AddHandler<T>(string name, IOperandHandler<IOperand> handler)
		{
			_handlers.Add(name, handler);
			return this;
		}

        public IMessageSet Validate(User searcher, IOperand operand, IWasClause clause)
        {
            IOperandHandler<IOperand> operandHandler;
            if (_handlers.TryGetValue(operand.Name, out operandHandler))
            {
                // return operandHandler.Validate(searcher, operand, clause.AsTerminalClause());
                return new MessageSet();
            }

            return new MessageSet();
        }

        public IEnumerable<QueryLiteral> GetValues(User searcher, IOperand operand, ITerminalClause terminalClause)
        {
            return GetValues(new QueryCreationContext(searcher, true), operand, terminalClause);
        }

        public IEnumerable<QueryLiteral> GetValues(IQueryCreationContext queryCreationContext, IOperand operand, ITerminalClause terminalClause)
        {
            IOperandHandler<IOperand> operandHandler;
            if (_handlers.TryGetValue(operand.Name, out operandHandler))
            {
                return operandHandler.GetValues(queryCreationContext, operand, terminalClause);
            }

            return null;
        }

        public IMessageSet Validate(User user, IOperand operand, ITerminalClause terminalClause)
        {
            IOperandHandler<IOperand> operandHandler;
            if (_handlers.TryGetValue(operand.Name, out operandHandler))
            {
                return operandHandler.Validate(user, operand, terminalClause);
            }

            return new MessageSet();
        }

        public FunctionOperand SanitizeFunctionOperand(User searcher, FunctionOperand operand)
        {
            return operand;
        }

        public QueryLiteral GetSingleValue(User user, IOperand operand, ITerminalClause clause)
        {
            var valuesList = GetValues(user, operand, clause).ToList();
            if (!valuesList.Any())
            {
                return null;
            }

            if (valuesList.Count() > 1)
            {
                throw new ArgumentOutOfRangeException(string.Format("Found more than one value in operand '{0}'; values were: {1}", operand, valuesList));
            }

            return valuesList.First();
        }

        public bool IsEmptyOperand(IOperand operand)
        {
            IOperandHandler<IOperand> operandHandler;
            if (_handlers.TryGetValue(operand.Name, out operandHandler))
            {
                return operandHandler != null && operandHandler.IsEmpty();
            }

            return false;
        }

        public bool IsFunctionOperand(IOperand operand)
        {
            IOperandHandler<IOperand> operandHandler;
            if (_handlers.TryGetValue(operand.Name, out operandHandler))
            {
                return operandHandler != null && operandHandler.IsFunction();
            }

            return false;
        }

        public bool IsListOperand(IOperand operand)
        {
            IOperandHandler<IOperand> operandHandler;
            if (_handlers.TryGetValue(operand.Name, out operandHandler))
            {
                return operandHandler != null && operandHandler.IsList();
            }

            return false;
        }

        public bool IsValidOperand(IOperand operand)
        {
            IOperandHandler<IOperand> operandHandler;
            if (_handlers.TryGetValue(operand.Name, out operandHandler))
            {
                return operandHandler != null;
            }

            return false;
        }

        public static MockJqlOperandResolver CreateSimpleSupport()
        {
            var support = new MockJqlOperandResolver();
            var handlers = new Dictionary<string, IOperandHandler<IOperand>>();
            handlers.Add("SingleValueOperand", new SingleValueOperandHandler());
            handlers.Add("MultiValueOperand", new MultiValueOperandHandler(support));
            handlers.Add("Empty", new EmptyOperandHandler());

            support.AddHandlers(handlers);
            return support;
        }
    }
}
