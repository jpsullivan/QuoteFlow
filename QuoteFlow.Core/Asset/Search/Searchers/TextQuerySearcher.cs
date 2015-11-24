using System;
using System.Collections.Generic;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Index.Indexers;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Search.Searchers;
using QuoteFlow.Api.Asset.Search.Searchers.Information;
using QuoteFlow.Api.Asset.Search.Searchers.Renderer;
using QuoteFlow.Api.Asset.Search.Searchers.Transformer;
using QuoteFlow.Api.Infrastructure.Concurrency;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Core.Asset.CustomFields.Searchers.Information;
using QuoteFlow.Core.Asset.Index.Indexers;
using QuoteFlow.Core.Asset.Search.Searchers.Renderer;
using QuoteFlow.Core.Asset.Search.Searchers.Transformer;

namespace QuoteFlow.Core.Asset.Search.Searchers
{
    /// <summary>
    /// Searcher for the multi-field text search input introduced as a gradual replacement 
    /// for the orignal QuerySearcher. This maps to the LHS of JQL query "text ~ foo" and 
    /// it searches for multiple fields.
    /// </summary>
    public class TextQuerySearcher : IAssetSearcher<ISearchableField>
    {
        private const string Id = "text";
        private const string NameKey = "common.words.query";

        public ISearcherInformation<ISearchableField> SearchInformation { get; set; }
        public ISearchInputTransformer SearchInputTransformer { get; set; }
        public ISearchRenderer SearchRenderer { get; set; }

        private static readonly IEnumerable<IFieldIndexer> ClassicFieldIndexers = new List<IFieldIndexer>
        {
            new DescriptionIndexer(),
            new SummaryIndexer()
        };

        public TextQuerySearcher(IJqlOperandResolver operandResolver)
        {
            var fieldRef = new AtomicReference<ISearchableField>();
            SearchInformation = new GenericSearcherInformation<ISearchableField>(Id, NameKey, ClassicFieldIndexers, fieldRef, SearcherGroupType.Text);
            SearchInputTransformer = new TextQuerySearchInputTransformer(Id, SystemSearchConstants.ForAllText(), operandResolver);
            SearchRenderer = new TextQuerySearchRenderer(Id, Id, NameKey, SearchInputTransformer);
        }

        public void Init(ISearchableField field)
        {
            // not supposed to have a field
            throw new NotSupportedException("This searcher not supposed to have a field");
        }
    }
}