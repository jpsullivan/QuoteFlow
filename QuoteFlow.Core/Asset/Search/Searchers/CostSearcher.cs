using System.Collections.Generic;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Index.Indexers;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Search.Searchers;
using QuoteFlow.Api.Asset.Search.Searchers.Information;
using QuoteFlow.Api.Asset.Search.Searchers.Renderer;
using QuoteFlow.Api.Asset.Search.Searchers.Transformer;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Core.Asset.CustomFields.Searchers.Information;
using QuoteFlow.Core.Asset.Index.Indexers;
using QuoteFlow.Core.Asset.Search.Searchers.Renderer;
using QuoteFlow.Core.Asset.Search.Searchers.Transformer;
using QuoteFlow.Core.Asset.Search.Searchers.Util;

namespace QuoteFlow.Core.Asset.Search.Searchers
{
    public sealed class CostSearcher : AbstractInitializationSearcher
    {
        private const string NameKey = "Cost";

        public override ISearcherInformation<ISearchableField> SearchInformation { get; set; }
        public override ISearchInputTransformer SearchInputTransformer { get; set; }
        public override ISearchRenderer SearchRenderer { get; set; }

        public CostSearcher(IJqlOperandResolver operandResolver)
        {
            var searchConstants = SystemSearchConstants.ForCost();
            var config = new CostSearcherConfig(searchConstants.JqlClauseNames.PrimaryName);

            SearchInformation = new GenericSearcherInformation<ISearchableField>(searchConstants.SearcherId, NameKey,
                new List<IFieldIndexer> {new CostIndexer()}, FieldReference, SearcherGroupType.Asset);
            SearchRenderer = new CostSearchRenderer(searchConstants, NameKey, config);
            SearchInputTransformer = new CostSearchInputTransformer(searchConstants, config, operandResolver);
        }
    }
}