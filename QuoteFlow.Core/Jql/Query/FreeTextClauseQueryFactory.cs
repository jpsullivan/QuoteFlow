using System.Collections.Generic;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;

namespace QuoteFlow.Core.Jql.Query
{
    /// <summary>
    /// A query factory that generates lucene queries for the text fields.
    /// </summary>
    public sealed class FreeTextClauseQueryFactory : IClauseQueryFactory
    {
        private readonly ClauseQueryFactory _delegateClauseQueryFactory;

        public FreeTextClauseQueryFactory(IJqlOperandResolver operandResolver, string documentConstant)
        {
            _delegateClauseQueryFactory = GetDelegate(operandResolver, documentConstant);
        }

        public QueryFactoryResult GetQuery(IQueryCreationContext queryCreationContext, ITerminalClause terminalClause)
        {
            return _delegateClauseQueryFactory.GetQuery(queryCreationContext, terminalClause);
        }

        private static ClauseQueryFactory GetDelegate(IJqlOperandResolver operandResolver, string documentConstant)
        {
            var operatorFactories = new List<IOperatorSpecificQueryFactory>();
            operatorFactories.Add(new LikeQueryFactory());
            return new GenericClauseQueryFactory(documentConstant, operatorFactories, operandResolver);
        }
    }
}