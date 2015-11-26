using System.Collections.Generic;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Resolver;
using QuoteFlow.Core.Jql.Resolver;

namespace QuoteFlow.Core.Jql.Query
{
    /// <summary>
    /// Clause query factory that creates the clauses for the creator field.
    /// Only supports equality operators.
    /// </summary>
    public class CreatorClauseQueryFactory : IClauseQueryFactory
    {
        private readonly IClauseQueryFactory _delegateClauseQueryFactory;

        public CreatorClauseQueryFactory(UserResolver userResolver, IJqlOperandResolver operandResolver)
        {
            var searchConstants = SystemSearchConstants.ForCreator();
            var operatorFactories = new List<IOperatorSpecificQueryFactory>();
            var indexInfoResolver = new UserIndexInfoResolver(userResolver);
        }

        public QueryFactoryResult GetQuery(IQueryCreationContext queryCreationContext, ITerminalClause terminalClause)
        {
            return _delegateClauseQueryFactory.GetQuery(queryCreationContext, terminalClause);
        }
    }
}