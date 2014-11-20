using System;
using System.Collections.Generic;
using QuoteFlow.Models.Assets.Fields;
using QuoteFlow.Models.Assets.Search.Constants;
using QuoteFlow.Models.Assets.Search.Searchers.Information;
using QuoteFlow.Models.Assets.Search.Searchers.Transformer;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Resolver;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Models.Assets.Search.Searchers
{
    /// <summary>
    /// Searcher for the catalog system field.
    /// </summary>
    public class CatalogSearcher : AbstractInitializationSearcher, IAssetSearcher<ISearchableField>
    {
        public CatalogSearcher(IJqlOperandResolver operandResolver, 
            CatalogResolver catalogResolver, ICatalogService catalogService, 
            FieldFlagOperandRegistry fieldFlagOperandRegistry)
        {
            var projectIndexInfoResolver = new CatalogIndexInfoResolver(catalogResolver);
            var constants = SystemSearchConstants.ForCatalog();

            SearchInputTransformer = new CatalogSearchInputTransformer(projectIndexInfoResolver, operandResolver, fieldFlagOperandRegistry, catalogService);
            SearchInformation = new GenericSearcherInformation<ISearchableField>(constants.SearcherId, "common.concepts.catalog", new List<Type>(typeof(CatalogIdIndexer)), fieldReference, SearcherGroupType.Context);
        }

        public virtual ISearcherInformation<ISearchableField> SearchInformation { get; private set; }

        public virtual ISearchInputTransformer SearchInputTransformer { get; private set; }
    }
}