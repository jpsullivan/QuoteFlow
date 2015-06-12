using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly IDictionary<string, object> _handlers;

        public MockJqlOperandResolver() : this(new Dictionary<string, object>())
		{
		}

		public MockJqlOperandResolver(IDictionary<string, object> handlers)
		{
			_handlers = handlers;
		}

        public virtual MockJqlOperandResolver AddHandlers(IDictionary<string, object> handlers)
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
            object operandHandler;
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
            object operandHandler;
            if (_handlers.TryGetValue(operand.Name, out operandHandler))
            {
                var svHandler = operandHandler as SingleValueOperandHandler;
                if (svHandler != null)
                {
                    return svHandler.GetValues(queryCreationContext, (SingleValueOperand) operand, terminalClause);
                }

                var mvHandler = operandHandler as MultiValueOperandHandler;
                if (mvHandler != null)
                {
                    return mvHandler.GetValues(queryCreationContext, (MultiValueOperand) operand, terminalClause);
                }

                var emptyHandler = operandHandler as EmptyOperandHandler;
                if (emptyHandler != null)
                {
                    return emptyHandler.GetValues(queryCreationContext, (EmptyOperand) operand, terminalClause);
                }

                var funcHandler = operandHandler as FunctionOperandHandler;
                if (funcHandler != null)
                {
                    return funcHandler.GetValues(queryCreationContext, (FunctionOperand) operand, terminalClause);
                }

                Debug.WriteLine("Could not resolve the operand handler: {0}", operand.Name);
                return null;
            }

            return null;
        }

        public IMessageSet Validate(User user, IOperand operand, ITerminalClause terminalClause)
        {
            object operandHandler;
            if (_handlers.TryGetValue(operand.Name, out operandHandler))
            {
                var svHandler = operandHandler as SingleValueOperandHandler;
                if (svHandler != null)
                {
                    return svHandler.Validate(user, operand as SingleValueOperand, terminalClause);
                }

                var mvHandler = operandHandler as MultiValueOperandHandler;
                if (mvHandler != null)
                {
                    return mvHandler.Validate(user, operand as MultiValueOperand, terminalClause);
                }

                var emptyHandler = operandHandler as EmptyOperandHandler;
                if (emptyHandler != null)
                {
                    return emptyHandler.Validate(user, operand as EmptyOperand, terminalClause);
                }

                var funcHandler = operandHandler as FunctionOperandHandler;
                if (funcHandler != null)
                {
                    return funcHandler.Validate(user, operand as FunctionOperand, terminalClause);
                }

                Debug.WriteLine("Could not resolve the operand handler: {0}", operand.Name);
                return null;
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
            object operandHandler;
            if (_handlers.TryGetValue(operand.Name, out operandHandler))
            {
                if (operandHandler == null)
                {
                    return false;
                }

                var svHandler = operandHandler as SingleValueOperandHandler;
                if (svHandler != null)
                {
                    return svHandler.IsEmpty();
                }

                var mvHandler = operandHandler as MultiValueOperandHandler;
                if (mvHandler != null)
                {
                    return mvHandler.IsEmpty();
                }

                var emptyHandler = operandHandler as EmptyOperandHandler;
                if (emptyHandler != null)
                {
                    return emptyHandler.IsEmpty();
                }

                var funcHandler = operandHandler as FunctionOperandHandler;
                if (funcHandler != null)
                {
                    return funcHandler.IsEmpty();
                }

                Debug.WriteLine("Could not resolve the operand handler: {0}", operand.Name);
            }

            return false;
        }

        public bool IsFunctionOperand(IOperand operand)
        {
            object operandHandler;
            if (_handlers.TryGetValue(operand.Name, out operandHandler))
            {
                if (operandHandler == null)
                {
                    return false;
                }

                var svHandler = operandHandler as SingleValueOperandHandler;
                if (svHandler != null)
                {
                    return svHandler.IsFunction();
                }

                var mvHandler = operandHandler as MultiValueOperandHandler;
                if (mvHandler != null)
                {
                    return mvHandler.IsFunction();
                }

                var emptyHandler = operandHandler as EmptyOperandHandler;
                if (emptyHandler != null)
                {
                    return emptyHandler.IsFunction();
                }

                var funcHandler = operandHandler as FunctionOperandHandler;
                if (funcHandler != null)
                {
                    return funcHandler.IsFunction();
                }

                Debug.WriteLine("Could not resolve the operand handler: {0}", operand.Name);
            }

            return false;
        }

        public bool IsListOperand(IOperand operand)
        {
            object operandHandler;
            if (_handlers.TryGetValue(operand.Name, out operandHandler))
            {
                if (operandHandler == null)
                {
                    return false;
                }

                var svHandler = operandHandler as SingleValueOperandHandler;
                if (svHandler != null)
                {
                    return svHandler.IsList();
                }

                var mvHandler = operandHandler as MultiValueOperandHandler;
                if (mvHandler != null)
                {
                    return mvHandler.IsList();
                }

                var emptyHandler = operandHandler as EmptyOperandHandler;
                if (emptyHandler != null)
                {
                    return emptyHandler.IsList();
                }

                var funcHandler = operandHandler as FunctionOperandHandler;
                if (funcHandler != null)
                {
                    return funcHandler.IsList();
                }

                Debug.WriteLine("Could not resolve the operand handler: {0}", operand.Name);
            }

            return false;
        }

        public bool IsValidOperand(IOperand operand)
        {
            object operandHandler;
            if (_handlers.TryGetValue(operand.Name, out operandHandler))
            {
                return operandHandler != null;
            }

            return false;
        }

        public static MockJqlOperandResolver CreateSimpleSupport()
        {
            var support = new MockJqlOperandResolver();
            var handlers = new Dictionary<string, object>();
            handlers.Add("SingleValueOperand", new SingleValueOperandHandler());
            handlers.Add("MultiValueOperand", new MultiValueOperandHandler(support));
            handlers.Add("Empty", new EmptyOperandHandler());

            support.AddHandlers(handlers);
            return support;
        }
    }
}
