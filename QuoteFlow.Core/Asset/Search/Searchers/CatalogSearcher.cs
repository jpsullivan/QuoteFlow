using System.Collections.Generic;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Index.Indexers;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Search.Searchers;
using QuoteFlow.Api.Asset.Search.Searchers.Information;
using QuoteFlow.Api.Asset.Search.Searchers.Renderer;
using QuoteFlow.Api.Asset.Search.Searchers.Transformer;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Asset.CustomFields.Searchers.Information;
using QuoteFlow.Core.Asset.Index.Indexers;
using QuoteFlow.Core.Asset.Search.Searchers.Renderer;
using QuoteFlow.Core.Jql.Resolver;
using CatalogSearchInputTransformer = QuoteFlow.Core.Asset.Search.Searchers.Transformer.CatalogSearchInputTransformer;

namespace QuoteFlow.Core.Asset.Search.Searchers
{
    /// <summary>
    /// Searcher for the catalog system field.
    /// </summary>
    public sealed class CatalogSearcher : AbstractInitializationSearcher
    {
        public override ISearcherInformation<ISearchableField> SearchInformation { get; set; }
        public override ISearchInputTransformer SearchInputTransformer { get; set; }
        public override ISearchRenderer SearchRenderer { get; set; }

        public CatalogSearcher(ICatalogService catalogService, 
            IJqlOperandResolver operandResolver, 
            CatalogResolver catalogResolver, 
            IFieldFlagOperandRegistry fieldFlagOperandRegistry)
        {
            var catalogIndexInfoResolver = new CatalogIndexInfoResolver(catalogResolver);
            var constants = SystemSearchConstants.ForCatalog();

            SearchInputTransformer = new CatalogSearchInputTransformer(catalogIndexInfoResolver, operandResolver, fieldFlagOperandRegistry, catalogService);
            SearchInformation = new GenericSearcherInformation<ISearchableField>(
                constants.SearcherId, 
                "Catalog",
                new List<IFieldIndexer> { new CatalogIdIndexer() }, 
                FieldReference, 
                SearcherGroupType.Context
            );
            SearchRenderer = new CatalogSearchRenderer(catalogService, SearchInformation.NameKey);
        }
    }
}