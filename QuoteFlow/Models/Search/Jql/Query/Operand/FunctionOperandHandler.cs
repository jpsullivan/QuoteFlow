using System;
using System.Collections.Generic;
using QuoteFlow.Infrastructure.Util;
using QuoteFlow.Models.Search.Jql.Function;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Query.Clause;

namespace QuoteFlow.Models.Search.Jql.Query.Operand
{
    /// <summary>
    /// Adapter to convert the plugin point <seealso cref="JqlFunction"/> into
    /// <seealso cref="OperandHandler"/>.
    /// </summary>
    public class FunctionOperandHandler : IOperandHandler<FunctionOperand>
	{
		protected internal readonly IJqlFunction jqlFunction;

		public FunctionOperandHandler(IJqlFunction jqlFunction)
		{
		    if (jqlFunction == null)
		    {
		        throw new ArgumentNullException("jqlFunction");
		    }

			this.jqlFunction = jqlFunction;
		}

		public virtual IMessageSet Validate(User searcher, FunctionOperand operand, ITerminalClause terminalClause)
		{
		}

        public IEnumerable<QueryLiteral> GetValues(QueryCreationContext queryCreationContext, FunctionOperand operand, ITerminalClause terminalClause)
        {
            throw new NotImplementedException();
        }

        public bool IsList()
        {
            throw new NotImplementedException();
        }

        public bool IsEmpty()
        {
            throw new NotImplementedException();
        }

        public bool IsFunction()
        {
            throw new NotImplementedException();
        }

        public virtual IList<QueryLiteral> GetValues(QueryCreationContext queryCreationContext, FunctionOperand operand, TerminalClause terminalClause)
        {
            return SafePluginPointAccess.call(new CallableAnonymousInnerClassHelper(this, queryCreationContext, operand, terminalClause)).getOrElse(Collections.emptyList<QueryLiteral>());
        }

        private class CallableAnonymousInnerClassHelper : Callable<IList<QueryLiteral>>
        {
            private readonly FunctionOperandHandler outerInstance;

            private QueryCreationContext queryCreationContext;
            private FunctionOperand operand;
            private TerminalClause terminalClause;

            public CallableAnonymousInnerClassHelper(FunctionOperandHandler outerInstance, QueryCreationContext queryCreationContext, FunctionOperand operand, TerminalClause terminalClause)
            {
                this.outerInstance = outerInstance;
                this.queryCreationContext = queryCreationContext;
                this.operand = operand;
                this.terminalClause = terminalClause;
            }

            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: @Override public java.util.List<QueryLiteral> call() throws Exception
            public override IList<QueryLiteral> call()
            {
                return outerInstance.jqlFunction.getValues(queryCreationContext, operand, terminalClause);
            }
        }

        public virtual bool List
        {
            get
            {
                return SafePluginPointAccess.call(new CallableAnonymousInnerClassHelper2(this)).getOrElse(false);
            }
        }

        private class CallableAnonymousInnerClassHelper2 : Callable<bool?>
        {
            private readonly FunctionOperandHandler outerInstance;

            public CallableAnonymousInnerClassHelper2(FunctionOperandHandler outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: @Override public Boolean call() throws Exception
            public override bool? call()
            {
                return outerInstance.jqlFunction.IsList;
            }
        }

        public virtual bool Empty
        {
            get { return false; }
        }

        public virtual bool Function
        {
            get { return true; }
        }

        public virtual IJqlFunction JqlFunction
        {
            get { return jqlFunction; }
        }

	}

}