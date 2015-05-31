using System.Collections.Generic;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Index.Indexers;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Search.Searchers;
using QuoteFlow.Api.Asset.Search.Searchers.Information;
using QuoteFlow.Api.Asset.Search.Searchers.Renderer;
using QuoteFlow.Api.Asset.Search.Searchers.Transformer;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Asset.CustomFields.Searchers.Information;
using QuoteFlow.Core.Asset.Fields;
using QuoteFlow.Core.Asset.Index.Indexers;
using QuoteFlow.Core.Asset.Search.Searchers.Renderer;
using QuoteFlow.Core.Jql.Resolver;

namespace QuoteFlow.Core.Asset.Search.Searchers
{
    /// <summary>
    /// Searcher for the <see cref="ManufacturerSystemField"/> system field.
    /// </summary>
    public sealed class ManufacturerSearcher : AbstractInitializationSearcher
    {
        public override ISearcherInformation<ISearchableField> SearchInformation { get; set; }
        public override ISearchInputTransformer SearchInputTransformer { get; set; }
        public override ISearchRenderer SearchRenderer { get; set; }

		public ManufacturerSearcher(ICatalogService catalogService, 
            IJqlOperandResolver operandResolver, 
            ManufacturerResolver manufacturerResolver, 
            FieldFlagOperandRegistry fieldFlagOperandRegistry)
		{
            var indexInfoResolver = new AssetConstantInfoResolver<Manufacturer>(manufacturerResolver);
            var constants = SystemSearchConstants.ForManufacturer();

		    SearchInformation = new GenericSearcherInformation<ISearchableField>(constants.SearcherId,
		        "Manufacturer", new List<IFieldIndexer> {new ManufacturerIndexer()},
		        FieldReference, SearcherGroupType.Context);
            SearchInputTransformer = new ManufacturerSearchInputTransformer(indexInfoResolver, operandResolver, fieldFlagOperandRegistry, manufacturerResolver);
		    SearchRenderer = new ManufacturerSearchRenderer(catalogService, constants, SearchInformation.NameKey);
		}
    }
}