using QuoteFlow.Infrastructure.Concurrency;
using QuoteFlow.Models.Assets.Fields;
using QuoteFlow.Models.Assets.Search.Searchers.Information;
using QuoteFlow.Models.Assets.Search.Searchers.Transformer;

namespace QuoteFlow.Models.Assets.Search.Searchers
{
    /// <summary>
    /// Abstract base class for searchers that performs the init methods for a searcher.
    /// 
    /// @since v4.0
    /// </summary>
    public abstract class AbstractInitializationSearcher : IAssetSearcher<ISearchableField>
    {
        protected internal readonly AtomicReference<ISearchableField> fieldReference;

        protected internal AbstractInitializationSearcher()
        {
            fieldReference = new AtomicReference<ISearchableField>(null);
        }

        public void Init(ISearchableField field)
        {
            fieldReference.Set(field);
        }

        public ISearcherInformation<ISearchableField> SearchInformation { get; private set; }
        public ISearchInputTransformer SearchInputTransformer { get; private set; }
    }
}