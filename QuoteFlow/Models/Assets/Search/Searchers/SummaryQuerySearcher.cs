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
            _fieldRef.Set(field);
        }
    }
}