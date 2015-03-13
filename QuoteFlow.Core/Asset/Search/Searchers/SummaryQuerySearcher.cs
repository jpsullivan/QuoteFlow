using System.Collections.Generic;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Index.Indexers;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Search.Searchers;
using QuoteFlow.Api.Asset.Search.Searchers.Information;
using QuoteFlow.Api.Asset.Search.Searchers.Transformer;
using QuoteFlow.Api.Infrastructure.Concurrency;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Core.Asset.CustomFields.Searchers.Information;
using QuoteFlow.Core.Asset.Index.Indexers;
using QuoteFlow.Core.Asset.Search.Searchers.Transformer;

namespace QuoteFlow.Core.Asset.Search.Searchers
{
    /// <summary>
    /// Searcher for the Summary field.
    /// This maps to the LHS of JQL query "summary ~ foo".
    /// </summary>
    public class SummaryQuerySearcher : IAssetSearcher<ISearchableField>
    {
        private const string NameKey = "asset.field.summary";

        public virtual ISearcherInformation<ISearchableField> SearchInformation { get; private set; }
        public virtual ISearchInputTransformer SearchInputTransformer { get; private set; }

        private static readonly IList<IFieldIndexer> SummaryIndexer = new List<IFieldIndexer> { new SummaryIndexer() };
        private readonly AtomicReference<ISearchableField> _fieldRef = new AtomicReference<ISearchableField>();

        public SummaryQuerySearcher(IJqlOperandResolver operandResolver)
        {
            var fieldInfo = SystemSearchConstants.ForSummary();
            SearchInformation = new GenericSearcherInformation<ISearchableField>(fieldInfo.SearcherId, NameKey, SummaryIndexer, _fieldRef, SearcherGroupType.Text);
            SearchInputTransformer = new TextQuerySearchInputTransformer(fieldInfo.SearcherId, fieldInfo, operandResolver);
        }

        public void Init(ISearchableField field)
        {
            _fieldRef.Value = field;
        }
    }
}