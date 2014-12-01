using System.Collections.Generic;
using QuoteFlow.Models.Assets.Fields;
using QuoteFlow.Models.Assets.Index.Indexers;
using QuoteFlow.Models.Assets.Search.Constants;
using QuoteFlow.Models.Assets.Search.Searchers.Information;
using QuoteFlow.Models.Assets.Search.Searchers.Transformer;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Resolver;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Models.Assets.Search.Searchers
{
    /// <summary>
    /// Searcher for the <see cref="ManufacturerSystemField"/> system field.
    /// </summary>
    public class ManufacturerSearcher : AbstractInitializationSearcher, IAssetSearcher<ISearchableField>
    {
        public virtual ISearcherInformation<ISearchableField> SearchInformation { get; private set; }
        public virtual ISearchInputTransformer SearchInputTransformer { get; private set; }

        public ManufacturerSearcher(IJqlOperandResolver operandResolver, ManufacturerResolver issueTypeResolver, IFieldFlagOperandRegistry fieldFlagOperandRegistry, ICatalogService projectManager)
		{
			var indexInfoResolver = new AssetConstantInfoResolver<Manufacturer>(issueTypeResolver);
			SimpleFieldSearchConstants constants = SystemSearchConstants.ForManufacturer();

			this.SearchInformation = new GenericSearcherInformation<ISearchableField>(constants.SearcherId, "navigator.filter.manufacturer", new List<IFieldIndexer> { new ManufacturerIndexer() }, base.FieldReference, SearcherGroupType.Context);
			this.SearchInputTransformer = new ManufacturerSearchInputTransformer(indexInfoResolver, operandResolver, fieldFlagOperandRegistry, issueTypeResolver);
		}
    }
}