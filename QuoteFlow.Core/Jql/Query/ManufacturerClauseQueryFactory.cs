using System.Collections.Generic;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Resolver;
using QuoteFlow.Api.Models;
using QuoteFlow.Core.Jql.Resolver;

namespace QuoteFlow.Core.Jql.Query
{
    /// <summary>
    /// Clause query factory that creates the clauses for the issue type field.
    /// Only supports equality operators.
    /// </summary>
    public class ManufacturerClauseQueryFactory : ClauseQueryFactory
    {
        private readonly ClauseQueryFactory delegateClauseQueryFactory;

        public ManufacturerClauseQueryFactory(ManufacturerResolver issueTypeResolver, IJqlOperandResolver operandResolver)
        {
            var operatorFactories = new List<IOperatorSpecificQueryFactory>();
            IIndexInfoResolver<Manufacturer> indexInfoResolver = new AssetConstantInfoResolver<Manufacturer>(issueTypeResolver);
            operatorFactories.Add(new EqualityQueryFactory<Manufacturer>(indexInfoResolver));
            delegateClauseQueryFactory = new GenericClauseQueryFactory(SystemSearchConstants.ForManufacturer(), operatorFactories, operandResolver);
        }

        public virtual QueryFactoryResult GetQuery(IQueryCreationContext queryCreationContext, ITerminalClause terminalClause)
        {
            return delegateClauseQueryFactory.GetQuery(queryCreationContext, terminalClause);
        }
    }
}