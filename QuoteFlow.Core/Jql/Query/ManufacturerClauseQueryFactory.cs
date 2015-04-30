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
    public class ManufacturerClauseQueryFactory : IClauseQueryFactory
    {
        private readonly IClauseQueryFactory _delegateClauseQueryFactory;

        public ManufacturerClauseQueryFactory(ManufacturerResolver issueTypeResolver, IJqlOperandResolver operandResolver)
        {
            var operatorFactories = new List<IOperatorSpecificQueryFactory>();
            IIndexInfoResolver<Manufacturer> indexInfoResolver = new AssetConstantInfoResolver<Manufacturer>(issueTypeResolver);
            operatorFactories.Add(new EqualityQueryFactory<Manufacturer>(indexInfoResolver));
            _delegateClauseQueryFactory = new GenericClauseQueryFactory(SystemSearchConstants.ForManufacturer(), operatorFactories, operandResolver);
        }

        public QueryFactoryResult GetQuery(IQueryCreationContext queryCreationContext, ITerminalClause terminalClause)
        {
            return _delegateClauseQueryFactory.GetQuery(queryCreationContext, terminalClause);
        }
    }
}