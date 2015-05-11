﻿using System.Collections.Generic;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Index.Indexers;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Search.Searchers;
using QuoteFlow.Api.Asset.Search.Searchers.Information;
using QuoteFlow.Api.Asset.Search.Searchers.Transformer;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Asset.CustomFields.Searchers.Information;
using QuoteFlow.Core.Asset.Fields;
using QuoteFlow.Core.Asset.Index.Indexers;
using QuoteFlow.Core.Jql.Resolver;

namespace QuoteFlow.Core.Asset.Search.Searchers
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
			var constants = SystemSearchConstants.ForManufacturer();

            SearchInformation = new GenericSearcherInformation<ISearchableField>(constants.SearcherId,
                "Manufacturer", new List<IFieldIndexer> {new ManufacturerIndexer()},
                FieldReference, SearcherGroupType.Context);

			SearchInputTransformer = new ManufacturerSearchInputTransformer(indexInfoResolver, operandResolver, fieldFlagOperandRegistry, issueTypeResolver);
		}
    }
}