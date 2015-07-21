using System.Collections.Generic;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;
using QuoteFlow.Core.Jql.Resolver;

namespace QuoteFlow.Core.Jql.Query
{
    /// <summary>
    /// A query factory that can generate queries for clauses that represent <see cref="Catalog"/>'s.
    /// </summary>
    public class CatalogClauseQueryFactory : IClauseQueryFactory
    {
        private readonly IClauseQueryFactory _delegateClauseQueryFactory;

        public CatalogClauseQueryFactory(CatalogResolver catalogResolver, IJqlOperandResolver operandResolver)
		{
			var catalogIndexInfoResolver = new CatalogIndexInfoResolver(catalogResolver);
            var operatorFactories = new List<IOperatorSpecificQueryFactory>
            {
                new EqualityQueryFactory<Catalog>(catalogIndexInfoResolver)
            };
            _delegateClauseQueryFactory = new GenericClauseQueryFactory(SystemSearchConstants.ForCatalog(), operatorFactories, operandResolver);
		}

        public QueryFactoryResult GetQuery(IQueryCreationContext queryCreationContext, ITerminalClause terminalClause)
        {
            return _delegateClauseQueryFactory.GetQuery(queryCreationContext, terminalClause);
        }
    }
}