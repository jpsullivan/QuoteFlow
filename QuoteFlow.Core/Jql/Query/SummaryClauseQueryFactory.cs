using System.Collections.Generic;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;

namespace QuoteFlow.Core.Jql.Query
{
    /// <summary>
    /// A clause query factory that handles the summary (asset name) system field.
    /// </summary>
    public sealed class SummaryClauseQueryFactory : IClauseQueryFactory
    {
        internal const int SummaryBoostFactor = 9;
        private readonly ClauseQueryFactory _delegateClauseQueryFactory;

        public SummaryClauseQueryFactory(IJqlOperandResolver operandResolver)
        {
            _delegateClauseQueryFactory = GetDelegate(operandResolver);
        }

        public QueryFactoryResult GetQuery(IQueryCreationContext queryCreationContext, ITerminalClause terminalClause)
        {
            QueryFactoryResult queryFactoryResult = _delegateClauseQueryFactory.GetQuery(queryCreationContext, terminalClause);
            if (queryFactoryResult != null && queryFactoryResult.LuceneQuery != null)
            {
                // Summary always gets a boost of 9
                queryFactoryResult.LuceneQuery.Boost = SummaryBoostFactor;
            }
            return queryFactoryResult;
        }

        private static ClauseQueryFactory GetDelegate(IJqlOperandResolver operandResolver)
        {
            var operatorFactories = new List<IOperatorSpecificQueryFactory>();
            operatorFactories.Add(new LikeQueryFactory());
            return new GenericClauseQueryFactory(SystemSearchConstants.ForSummary(), operatorFactories, operandResolver);
        }
    }
}