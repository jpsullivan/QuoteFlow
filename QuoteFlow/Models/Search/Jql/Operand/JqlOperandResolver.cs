﻿using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Infrastructure.Extensions;
using QuoteFlow.Infrastructure.Util;
using QuoteFlow.Models.Assets.Search;
using QuoteFlow.Models.Search.Jql.Function;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Query.Operand;
using QuoteFlow.Models.Search.Jql.Query.Operand.Registry;

namespace QuoteFlow.Models.Search.Jql.Operand
{
    /// <summary>
    /// Default implementation of the <see cref="IJqlOperandResolver"/> interface.
    /// </summary>
    public class JqlOperandResolver : IJqlOperandResolver
    {
		private readonly IJqlFunctionHandlerRegistry registry;
		private readonly IOperandHandler<IOperand> emptyHandler;
        private readonly IOperandHandler<IOperand> singleHandler;
        private readonly IOperandHandler<IOperand> multiHandler;
		// this is a request scoped cache
		private readonly IQueryCache queryCache;

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

		    this.registry = registry;
			this.queryCache = queryCache;
			this.emptyHandler = new EmptyOperandHandler();
			this.singleHandler = new SingleValueOperandHandler();
			this.multiHandler = new MultiValueOperandHandler(this);
		}

        internal JqlOperandResolver(IJqlFunctionHandlerRegistry registry, IOperandHandler<IOperand> emptyHandler, IOperandHandler<IOperand> singleHandler, IOperandHandler<IOperand> multiHandler, IQueryCache queryCache)
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

			this.registry = registry;
			this.emptyHandler = emptyHandler;
			this.singleHandler = singleHandler;
			this.multiHandler = multiHandler;
			this.queryCache = queryCache;
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
                values = emptyHandler.GetValues(queryCreationContext, operand, terminalClause);
            }
            else if (operand is SingleValueOperand)
            {
                values = singleHandler.GetValues(queryCreationContext, operand, terminalClause);
            }
            else if (operand is MultiValueOperand)
            {
                values = multiHandler.GetValues(queryCreationContext, operand, terminalClause);
            }
            else if (operand is FunctionOperand)
            {
                var funcOperand = (FunctionOperand) operand;
                var handler = registry.GetOperandHandler(funcOperand);
                values = handler != null ? handler.GetValues(queryCreationContext, funcOperand, terminalClause) : new List<QueryLiteral>();
            }
            else
            {
                //log.debug(string.Format("Unknown operand type '{0}' with name '{1}'", operand.GetType(), operand.DisplayString));
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

            if (operand is EmptyOperand)
            {
                return emptyHandler.Validate(user, operand, terminalClause);
            }
            if (operand is SingleValueOperand)
            {
                return singleHandler.Validate(user, operand, terminalClause);
            }
            if (operand is MultiValueOperand)
            {
                return multiHandler.Validate(user, operand, terminalClause);
            }
            if (operand is FunctionOperand)
            {
                var funcOperand = (FunctionOperand) operand;
                var handler = registry.GetOperandHandler(funcOperand);
                if (handler != null)
                {
                    return handler.Validate(user, funcOperand, terminalClause);
                }
                var messageSet = new MessageSet();
                messageSet.AddErrorMessage(string.Format("jira.jql.operand.illegal.function: {0}", operand.DisplayString));
                return messageSet;
            }
            else
            {
                //log.debug(string.Format("Unknown operand type '{0}' with name '{1}'", operand.GetType(), operand.DisplayString));

                var messageSet = new MessageSet();
                messageSet.AddErrorMessage(string.Format("jira.jql.operand.illegal.operand: {0}", operand.DisplayString));
                return messageSet;
            }
        }

        public FunctionOperand SanitizeFunctionOperand(User searcher, FunctionOperand operand)
        {
            FunctionOperandHandler funcHandler = registry.GetOperandHandler(operand);
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
            IOperandHandler<IOperand> operandHandler = GetOperandHandler(operand);
			return operandHandler != null && operandHandler.IsEmpty();
        }

        public bool IsFunctionOperand(IOperand operand)
        {
            IOperandHandler<IOperand> handler = GetOperandHandler(operand);
			return handler != null && handler.IsFunction();
        }

        public bool IsListOperand(IOperand operand)
        {
            IOperandHandler<IOperand> handler = GetOperandHandler(operand);
			return handler != null && handler.IsList();
        }

        public bool IsValidOperand(IOperand operand)
        {
            return GetOperandHandler(operand) != null;
        }

        private IOperandHandler<IOperand> GetOperandHandler(IOperand operand)
		{
            if (operand == null)
            {
                throw new ArgumentNullException("operand");
            }

			if (operand is EmptyOperand)
			{
				return emptyHandler;
			}
            if (operand is SingleValueOperand)
            {
                return singleHandler;
            }
            if (operand is MultiValueOperand)
            {
                return multiHandler;
            }
            if (operand is FunctionOperand)
            {
                var funcOperand = (FunctionOperand) operand;
                return registry.GetOperandHandler(funcOperand);
            }
            
            //log.debug(string.Format("Unknown operand type '{0}' with name '{1}'", operand.GetType(), operand.DisplayString));
            return null;
		}

        private IEnumerable<QueryLiteral> GetValuesFromCache(IQueryCreationContext ctx, IOperand operand, ITerminalClause jqlClause)
        {
            return queryCache.GetValues(ctx, operand, jqlClause);
        }


        private void PutValuesInCache(IQueryCreationContext ctx, IOperand operand, ITerminalClause jqlClause, IEnumerable<QueryLiteral> values)
        {
            if (values != null)
            {
                queryCache.SetValues(ctx, operand, jqlClause, values);
            }
        }
    }
}