using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuoteFlow.Infrastructure.Util;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Query.Operand;

namespace QuoteFlow.Models.Search.Jql.Operand
{
    /// <summary>
    /// Default implementation of the <see cref="IJqlOperandResolver"/> interface.
    /// </summary>
    public class JqlOperandResolver : IJqlOperandResolver
    {
		private readonly JqlFunctionHandlerRegistry registry;
		private readonly OperandHandler<EmptyOperand> emptyHandler;
		private readonly OperandHandler<SingleValueOperand> singleHandler;
		private readonly OperandHandler<MultiValueOperand> multiHandler;
		// this is a request scoped cache
		private readonly QueryCache queryCache;

		public JqlOperandResolver(JqlFunctionHandlerRegistry registry, QueryCache queryCache)
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

        internal JqlOperandResolver(JqlFunctionHandlerRegistry registry, OperandHandler<EmptyOperand> emptyHandler, OperandHandler<SingleValueOperand> singleHandler, OperandHandler<MultiValueOperand> multiHandler, QueryCache queryCache)
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

        public string Validate(User searcher, IOperand operand, IWasClause clause)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<QueryLiteral> GetValues(User searcher, IOperand operand, ITerminalClause terminalClause)
        {
            throw new NotImplementedException();
        }

        public IList<QueryLiteral> GetValues(IQueryCreationContext queryCreationContext, IOperand operand, ITerminalClause terminalClause)
        {
            throw new NotImplementedException();
        }

        public IMessageSet Validate(User user, IOperand operand, ITerminalClause terminalClause)
        {
            throw new NotImplementedException();
        }

        public FunctionOperand SanitiseFunctionOperand(User searcher, FunctionOperand operand)
        {
            throw new NotImplementedException();
        }

        public QueryLiteral GetSingleValue(User user, IOperand operand, ITerminalClause clause)
        {
            throw new NotImplementedException();
        }

        public bool IsEmptyOperand(IOperand operand)
        {
            throw new NotImplementedException();
        }

        public bool IsFunctionOperand(IOperand operand)
        {
            throw new NotImplementedException();
        }

        public bool IsListOperand(IOperand operand)
        {
            throw new NotImplementedException();
        }

        public bool IsValidOperand(IOperand operand)
        {
            throw new NotImplementedException();
        }
    }
}