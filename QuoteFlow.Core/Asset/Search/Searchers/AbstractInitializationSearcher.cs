using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Search.Searchers;
using QuoteFlow.Api.Asset.Search.Searchers.Information;
using QuoteFlow.Api.Asset.Search.Searchers.Renderer;
using QuoteFlow.Api.Asset.Search.Searchers.Transformer;
using QuoteFlow.Api.Infrastructure.Concurrency;

namespace QuoteFlow.Core.Asset.Search.Searchers
{
    /// <summary>
    /// Abstract base class for searchers that performs the init methods for a searcher.
    /// </summary>
    public abstract class AbstractInitializationSearcher : IAssetSearcher<ISearchableField>
    {
        protected readonly AtomicReference<ISearchableField> FieldReference;

        protected internal AbstractInitializationSearcher()
        {
            FieldReference = new AtomicReference<ISearchableField>();
        }

        public void Init(ISearchableField field)
        {
            FieldReference.Value = field;
        }

        public abstract ISearcherInformation<ISearchableField> SearchInformation { get; set; }
        public abstract ISearchInputTransformer SearchInputTransformer { get; set; }
        public abstract ISearchRenderer SearchRenderer { get; set; }
    }
}