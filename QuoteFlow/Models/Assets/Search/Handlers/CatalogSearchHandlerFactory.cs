using QuoteFlow.Models.Assets.Search.Constants;
using QuoteFlow.Models.Assets.Search.Searchers;
using QuoteFlow.Models.Search.Jql.Context;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Resolver;
using QuoteFlow.Models.Search.Jql.Validator;

namespace QuoteFlow.Models.Assets.Search.Handlers
{
    public sealed class CatalogSearchHandlerFactory : SimpleSearchHandlerFactory
    {
        public CatalogSearchHandlerFactory(
            CatalogClauseQueryFactory clauseFactory, 
            CatalogValidator caluseValidator,
            IJqlOperandResolver jqlOperandResolver, 
            CatalogResolver catalogResolver, 
            MultiClauseDecoratorContextFactory.Factory multiFactory)
            : base(SystemSearchConstants.ForCatalog(), typeof(CatalogSearcher), clauseFactory, caluseValidator, 
            multiFactory.Create(new CatalogClauseContextFactory(jqlOperandResolver, catalogResolver)), 
            new CatalogClauseValuesGenerator(), 
            new CatalogClauseValueSanitizer(jqlOperandResolver, catalogResolver))
        {
        }
    }
}