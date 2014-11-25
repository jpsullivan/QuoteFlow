using System.Collections.Generic;
using QuoteFlow.Models.Assets.Search.Constants;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Resolver;

namespace QuoteFlow.Models.Search.Jql.Query
{
    /// <summary>
    /// A query factory that can generate queries for clauses that represent <see cref="Catalog"/>'s.
    /// </summary>
    public class CatalogClauseQueryFactory : IClauseQueryFactory
    {
        private readonly ClauseQueryFactory delegateClauseQueryFactory;

        public CatalogClauseQueryFactory(CatalogResolver catalogResolver, IJqlOperandResolver operandResolver)
		{
			var catalogIndexInfoResolver = new CatalogIndexInfoResolver(catalogResolver);
			var operatorFactories = new List<IOperatorSpecificQueryFactory>();
			operatorFactories.Add(new EqualityQueryFactory<Catalog>(catalogIndexInfoResolver));
			delegateClauseQueryFactory = new GenericClauseQueryFactory(SystemSearchConstants.ForCatalog(), operatorFactories, operandResolver);
		}

        public QueryFactoryResult GetQuery(IQueryCreationContext queryCreationContext, ITerminalClause terminalClause)
        {
            return delegateClauseQueryFactory.GetQuery(queryCreationContext, terminalClause);
        }
    }
}