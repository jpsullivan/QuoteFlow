using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Jql.Function;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Jql.Query.Operand.Registry;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;
using QuoteFlow.Core.Asset.Search;

namespace QuoteFlow.Core.Jql.Operand
{
    /// <summary>
    /// Default implementation of the <see cref="IJqlOperandResolver"/> interface.
    /// </summary>
    public class JqlOperandResolver : IJqlOperandResolver
    {
		private readonly IJqlFunctionHandlerRegistry _registry;
		private readonly IOperandHandler<EmptyOperand> _emptyHandler;
        private readonly IOperandHandler<SingleValueOperand> _singleHandler;
        private readonly IOperandHandler<MultiValueOperand> _multiHandler;
		// this is a request scoped cache
		private readonly IQueryCache _queryCache;

        public JqlOperandResolver(IJqlFunctionHandlerRegistry registry, IQueryCache queryCache)
		{
		    if (registry == null)
		    {
		        throw new ArgumentNullException("registry");
		    }

		    if (queryCache == null)
		    {
		        throw new ArgumentNullException("queryCache");
		    }

		    _registry = registry;
			_queryCache = queryCache;
			_emptyHandler = new EmptyOperandHandler();
			_singleHandler = new SingleValueOperandHandler();
			_multiHandler = new MultiValueOperandHandler(this);
		}

        public JqlOperandResolver(IJqlFunctionHandlerRegistry registry, IOperandHandler<EmptyOperand> emptyHandler, IOperandHandler<SingleValueOperand> singleHandler, IOperandHandler<MultiValueOperand> multiHandler, IQueryCache queryCache)
		{
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }

            if (emptyHandler == null)
            {
                throw new ArgumentNullException("emptyHandler");
            }

            if (singleHandler == null)
            {
                throw new ArgumentNullException("singleHandler");
            }

            if (multiHandler == null)
            {
                throw new ArgumentNullException("multiHandler");
            }

            if (queryCache == null)
            {
                throw new ArgumentNullException("queryCache");
            }

			_registry = registry;
			_emptyHandler = emptyHandler;
			_singleHandler = singleHandler;
			_multiHandler = multiHandler;
			_queryCache = queryCache;
		}

        IMessageSet IJqlOperandResolver.Validate(User searcher, IOperand operand, IWasClause clause)
        {
            return new MessageSet();
        }

        public IEnumerable<QueryLiteral> GetValues(User searcher, IOperand operand, ITerminalClause terminalClause)
        {
            return GetValues(new QueryCreationContext(searcher), operand, terminalClause);
        }

        public IEnumerable<QueryLiteral> GetValues(IQueryCreationContext queryCreationContext, IOperand operand, ITerminalClause terminalClause)
        {
            if (operand == null)
            {
                throw new ArgumentNullException("operand");
            }

            var values = GetValuesFromCache(queryCreationContext, operand, terminalClause);
            if (values != null) return values;

            if (operand is EmptyOperand)
            {
                values = _emptyHandler.GetValues(queryCreationContext, (EmptyOperand) operand, terminalClause);
            }
            else if (operand is SingleValueOperand)
            {
                values = _singleHandler.GetValues(queryCreationContext, (SingleValueOperand) operand, terminalClause);
            }
            else if (operand is MultiValueOperand)
            {
                values = _multiHandler.GetValues(queryCreationContext, (MultiValueOperand) operand, terminalClause);
            }
            else if (operand is FunctionOperand)
            {
                var funcOperand = (FunctionOperand) operand;
                var handler = _registry.GetOperandHandler(funcOperand);
                values = handler != null ? handler.GetValues(queryCreationContext, funcOperand, terminalClause) : new List<QueryLiteral>();
            }
            else
            {
                Debug.WriteLine("Unknown operand type '{0}' with name '{1}'", operand.GetType(), operand.DisplayString);
            }
            
            PutValuesInCache(queryCreationContext, operand, terminalClause, values);
            return values;

        }

        public IMessageSet Validate(User user, IOperand operand, ITerminalClause terminalClause)
        {
            if (operand == null)
            {
                throw new ArgumentNullException("operand");
            }

            var emptyOperand = operand as EmptyOperand;
            if (emptyOperand != null)
            {
                return _emptyHandler.Validate(user, emptyOperand, terminalClause);
            }

            var valueOperand = operand as SingleValueOperand;
            if (valueOperand != null)
            {
                return _singleHandler.Validate(user, valueOperand, terminalClause);
            }

            var multiValueOperand = operand as MultiValueOperand;
            if (multiValueOperand != null)
            {
                return _multiHandler.Validate(user, multiValueOperand, terminalClause);
            }

            var functionOperand = operand as FunctionOperand;
            if (functionOperand != null)
            {
                var funcOperand = functionOperand;
                var handler = _registry.GetOperandHandler(funcOperand);
                if (handler != null)
                {
                    return handler.Validate(user, funcOperand, terminalClause);
                }
                var messageSet = new MessageSet();
                messageSet.AddErrorMessage(string.Format("quoteflow.jql.operand.illegal.function: {0}", operand.DisplayString));
                return messageSet;
            }
            else
            {
                //log.debug(string.Format("Unknown operand type '{0}' with name '{1}'", operand.GetType(), operand.DisplayString));

                var messageSet = new MessageSet();
                messageSet.AddErrorMessage(string.Format("quoteflow.jql.operand.illegal.operand: {0}", operand.DisplayString));
                return messageSet;
            }
        }

        public FunctionOperand SanitizeFunctionOperand(User searcher, FunctionOperand operand)
        {
            FunctionOperandHandler funcHandler = _registry.GetOperandHandler(operand);
            if (funcHandler == null) return operand;
            IJqlFunction jqlFunction = funcHandler.JqlFunction;
            var function = jqlFunction as IClauseSanitizingJqlFunction;
            if (function != null)
            {
                return function.SanitizeOperand(searcher, operand);
            }

            return operand;
        }

        public QueryLiteral GetSingleValue(User user, IOperand operand, ITerminalClause clause)
        {
            var list = GetValues(user, operand, clause);
            if (!list.AnySafe())
            {
                return null;
            }
            if (list.Count() > 1)
            {
                throw new ArgumentException("Found more than one value in operand '" + operand + "'; values were: " + list);
            }

            return list.First();
        }

        public bool IsEmptyOperand(IOperand operand)
        {
            var handler = GetOperandHandler(operand);

            if (handler == null)
            {
                return false;
            }

            if (handler is EmptyOperandHandler)
            {
                var resolvedHandler = handler as EmptyOperandHandler;
                return resolvedHandler.IsEmpty();
            }
            if (handler is SingleValueOperandHandler)
            {
                var resolvedHandler = handler as SingleValueOperandHandler;
                return resolvedHandler.IsEmpty();
            }
            if (handler is MultiValueOperandHandler)
            {
                var resolvedHandler = handler as MultiValueOperandHandler;
                return resolvedHandler.IsEmpty();
            }
            if (operand is FunctionOperandHandler)
            {
                var resolvedHandler = handler as FunctionOperandHandler;
                return resolvedHandler != null && resolvedHandler.IsEmpty();
            }

            return false;
        }

        public bool IsFunctionOperand(IOperand operand)
        {
            var handler = GetOperandHandler(operand);

            if (handler == null)
            {
                return false;
            }

            if (handler is EmptyOperandHandler)
            {
                var resolvedHandler = handler as EmptyOperandHandler;
                return resolvedHandler.IsFunction();
            }
            if (handler is SingleValueOperandHandler)
            {
                var resolvedHandler = handler as SingleValueOperandHandler;
                return resolvedHandler.IsFunction();
            }
            if (handler is MultiValueOperandHandler)
            {
                var resolvedHandler = handler as MultiValueOperandHandler;
                return resolvedHandler.IsFunction();
            }
            if (operand is FunctionOperandHandler)
            {
                var resolvedHandler = handler as FunctionOperandHandler;
                return resolvedHandler != null && resolvedHandler.IsFunction();
            }

            return false;
        }

        public bool IsListOperand(IOperand operand)
        {
            var handler = GetOperandHandler(operand);

            if (handler == null)
            {
                return false;
            }

            if (handler is EmptyOperandHandler)
            {
                var resolvedHandler = handler as EmptyOperandHandler;
                return resolvedHandler.IsList();
            }
            if (handler is SingleValueOperandHandler)
            {
                var resolvedHandler = handler as SingleValueOperandHandler;
                return resolvedHandler.IsList();
            }
            if (handler is MultiValueOperandHandler)
            {
                var resolvedHandler = handler as MultiValueOperandHandler;
                return resolvedHandler.IsList();
            }
            if (operand is FunctionOperandHandler)
            {
                var resolvedHandler = handler as FunctionOperandHandler;
                return resolvedHandler != null && resolvedHandler.IsList();
            }

            return false;
        }

        public bool IsValidOperand(IOperand operand)
        {
            return GetOperandHandler(operand) != null;
        }

        private object GetOperandHandler(IOperand operand)
		{
            if (operand == null)
            {
                throw new ArgumentNullException("operand");
            }

			if (operand is EmptyOperand)
			{
				return _emptyHandler;
			}
            if (operand is SingleValueOperand)
            {
                return _singleHandler;
            }
            if (operand is MultiValueOperand)
            {
                return _multiHandler;
            }
            if (operand is FunctionOperand)
            {
                var funcOperand = (FunctionOperand) operand;
                return _registry.GetOperandHandler(funcOperand);
            }

            Debug.WriteLine("Unknown operand type '{0}' with name '{1}'", operand.GetType(), operand.DisplayString);
            return null;
		}

        private IEnumerable<QueryLiteral> GetValuesFromCache(IQueryCreationContext ctx, IOperand operand, ITerminalClause jqlClause)
        {
            return _queryCache.GetValues(ctx, operand, jqlClause);
        }


        private void PutValuesInCache(IQueryCreationContext ctx, IOperand operand, ITerminalClause jqlClause, IEnumerable<QueryLiteral> values)
        {
            if (values != null)
            {
                _queryCache.SetValues(ctx, operand, jqlClause, values.ToList());
            }
        }
    }
}