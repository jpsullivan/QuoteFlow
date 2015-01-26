using System.Collections.Generic;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Index.Indexers;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Search.Searchers;
using QuoteFlow.Api.Asset.Search.Searchers.Information;
using QuoteFlow.Api.Asset.Search.Searchers.Transformer;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Jql.Resolver;
using CatalogSearchInputTransformer = QuoteFlow.Core.Asset.Search.Searchers.Transformer.CatalogSearchInputTransformer;

namespace QuoteFlow.Core.Asset.Search.Searchers
{
    /// <summary>
    /// Searcher for the catalog system field.
    /// </summary>
    public class CatalogSearcher : AbstractInitializationSearcher, IAssetSearcher<ISearchableField>
    {
        public CatalogSearcher(IJqlOperandResolver operandResolver, 
            CatalogResolver catalogResolver, ICatalogService catalogService, 
            IFieldFlagOperandRegistry fieldFlagOperandRegistry)
        {
            var projectIndexInfoResolver = new CatalogIndexInfoResolver(catalogResolver);
            var constants = SystemSearchConstants.ForCatalog();

            SearchInputTransformer = new CatalogSearchInputTransformer(projectIndexInfoResolver, operandResolver, fieldFlagOperandRegistry, catalogService);
            SearchInformation = new GenericSearcherInformation<ISearchableField>(
                constants.SearcherId, 
                "common.concepts.catalog",
                new List<IFieldIndexer> { new CatalogIdIndexer() }, 
                FieldReference, 
                SearcherGroupType.Context
            );
        }

        public virtual ISearcherInformation<ISearchableField> SearchInformation { get; private set; }

        public virtual ISearchInputTransformer SearchInputTransformer { get; private set; }
    }
}