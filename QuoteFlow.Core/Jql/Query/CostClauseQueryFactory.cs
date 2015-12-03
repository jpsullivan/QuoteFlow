using System.Collections.Generic;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Core.Jql.Resolver;

namespace QuoteFlow.Core.Jql.Query
{
    /// <summary>
    /// Creates queries for the cost system field clauses.
    /// </summary>
    public class CostClauseQueryFactory : IClauseQueryFactory
    {
        private readonly IClauseQueryFactory _delegateClauseQueryFactory;

        public CostClauseQueryFactory(IJqlOperandResolver operandResolver)
        {
            var operatorFactories = new List<IOperatorSpecificQueryFactory>();
            var costIndexInfoResolver = new CostIndexInfoResolver();
            operatorFactories.Add(new EqualityQueryFactory<object>(costIndexInfoResolver));
            operatorFactories.Add(new RelationalOperatorMutatedIndexValueQueryFactory<object>(costIndexInfoResolver));
            _delegateClauseQueryFactory = CreateGenericClauseQueryFactory(operandResolver, operatorFactories);
        }

        public QueryFactoryResult GetQuery(IQueryCreationContext queryCreationContext, ITerminalClause terminalClause)
        {
            return _delegateClauseQueryFactory.GetQuery(queryCreationContext, terminalClause);
        }

        private static GenericClauseQueryFactory CreateGenericClauseQueryFactory(IJqlOperandResolver operandResolver,
            IEnumerable<IOperatorSpecificQueryFactory> operatorFactories)
        {
            return new GenericClauseQueryFactory(SystemSearchConstants.ForCost(), operatorFactories, operandResolver);
        }
    }
}