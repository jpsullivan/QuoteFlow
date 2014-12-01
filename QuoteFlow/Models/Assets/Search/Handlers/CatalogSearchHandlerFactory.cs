using QuoteFlow.Models.Assets.Search.Constants;
using QuoteFlow.Models.Assets.Search.Searchers;
using QuoteFlow.Models.Search.Jql.Context;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Permission;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Resolver;
using QuoteFlow.Models.Search.Jql.Validator;
using QuoteFlow.Models.Search.Jql.Values;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Models.Assets.Search.Handlers
{
    public sealed class CatalogSearchHandlerFactory : SimpleSearchHandlerFactory
    {
        public CatalogSearchHandlerFactory(
            CatalogClauseQueryFactory clauseFactory, CatalogValidator caluseValidator, 
            ICatalogService catalogService, IJqlOperandResolver jqlOperandResolver, 
            CatalogResolver catalogResolver, MultiClauseDecoratorContextFactory.Factory multiFactory
        ) : base(SystemSearchConstants.ForCatalog(), typeof(CatalogSearcher), clauseFactory, caluseValidator, null,
                multiFactory.Create(new CatalogClauseContextFactory(catalogService, jqlOperandResolver, catalogResolver)), 
                new CatalogClauseValuesGenerator(catalogService),
                new CatalogClauseValueSanitizer(catalogService, jqlOperandResolver, catalogResolver))
        {
        }
    }
}