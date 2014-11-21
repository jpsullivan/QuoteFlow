using System;
using System.Collections.Generic;
using QuoteFlow.Infrastructure.Concurrency;
using QuoteFlow.Models.Assets.Fields;
using QuoteFlow.Models.Assets.Index.Indexers;
using QuoteFlow.Models.Assets.Search.Constants;
using QuoteFlow.Models.Assets.Search.Searchers.Information;
using QuoteFlow.Models.Assets.Search.Searchers.Transformer;
using QuoteFlow.Models.Search.Jql.Operand;

namespace QuoteFlow.Models.Assets.Search.Searchers
{
    /// <summary>
    /// Searcher for the multi-field text search input introduced 2012 as a gradual replacement for the orignal QuerySearcher.
    /// This maps to the LHS of JQL query "text ~ foo" and it searches for multiple fields.
    /// </summary>
    public class TextQuerySearcher : IAssetSearcher<ISearchableField>
    {
        private const string ID = "text";
        private const string NAME_KEY = "common.words.query";

        private static readonly IEnumerable<IFieldIndexer> CLASSIC_FIELD_INDEXERS = new List<IFieldIndexer>
        {
            new DescriptionIndexer(),
            new SummaryIndexer()
        };

        private readonly ISearcherInformation<ISearchableField> searcherInformation;
        private readonly ISearchInputTransformer searchInputTransformer;

        public TextQuerySearcher(IJqlOperandResolver operandResolver)
        {
            var fieldRef = new AtomicReference<ISearchableField>();
            SearchInformation = new GenericSearcherInformation<ISearchableField>(ID, NAME_KEY, CLASSIC_FIELD_INDEXERS, fieldRef, SearcherGroupType.Text);
            SearchInputTransformer = new TextQuerySearchInputTransformer(ID, SystemSearchConstants.ForAllText(), operandResolver);
        }

        public void Init(ISearchableField field)
        {
            // not supposed to have a field
            throw new NotSupportedException("This searcher not supposed to have a field");
        }

        public ISearcherInformation<ISearchableField> SearchInformation { get; private set; }
        public ISearchInputTransformer SearchInputTransformer { get; private set; }
    }
}