using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Asset.Search.Handlers;
using QuoteFlow.Core.Asset.Search.Searchers;
using QuoteFlow.Core.Jql.Context;
using QuoteFlow.Core.Jql.Permission;
using QuoteFlow.Core.Jql.Query;
using QuoteFlow.Core.Jql.Resolver;
using QuoteFlow.Core.Jql.Validator;
using QuoteFlow.Core.Jql.Values;

namespace QuoteFlow.Core.Asset.CustomFields.Searchers.Transformer
{
    public sealed class CatalogSearchHandlerFactory : SimpleSearchHandlerFactory
    {
        public CatalogSearchHandlerFactory(
            CatalogClauseQueryFactory clauseFactory, CatalogValidator caluseValidator, 
            ICatalogService catalogService, IJqlOperandResolver jqlOperandResolver,
            FieldClausePermissionChecker.IFactory clausePermissionFactory, CatalogResolver catalogResolver, MultiClauseDecoratorContextFactory.Factory multiFactory
        ) : base(SystemSearchConstants.ForCatalog(), typeof(CatalogSearcher), clauseFactory, caluseValidator, clausePermissionFactory,
                multiFactory.Create(new CatalogClauseContextFactory(catalogService, jqlOperandResolver, catalogResolver)), 
                new CatalogClauseValuesGenerator(catalogService),
                new CatalogClauseValueSanitizer(catalogService, jqlOperandResolver, catalogResolver))
        {
        }
    }
}