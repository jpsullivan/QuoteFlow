using QuoteFlow.Infrastructure.Concurrency;
using QuoteFlow.Models.Assets.Fields;
using QuoteFlow.Models.Assets.Search.Searchers.Information;
using QuoteFlow.Models.Assets.Search.Searchers.Transformer;

namespace QuoteFlow.Models.Assets.Search.Searchers
{
    /// <summary>
    /// Abstract base class for searchers that performs the init methods for a searcher.
    /// </summary>
    public abstract class AbstractInitializationSearcher : IAssetSearcher<ISearchableField>
    {
        protected AtomicReference<ISearchableField> FieldReference;

        protected internal AbstractInitializationSearcher()
        {
            FieldReference = new AtomicReference<ISearchableField>();
        }

        public void Init(ISearchableField field)
        {
            FieldReference.Set(field);
        }

        public ISearcherInformation<ISearchableField> SearchInformation { get; private set; }
        public ISearchInputTransformer SearchInputTransformer { get; private set; }
    }
}