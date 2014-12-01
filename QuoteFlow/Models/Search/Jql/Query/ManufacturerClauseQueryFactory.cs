using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuoteFlow.Models.Assets.Search.Constants;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Resolver;

namespace QuoteFlow.Models.Search.Jql.Query
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