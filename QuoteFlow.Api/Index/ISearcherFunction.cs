using Lucene.Net.Search;

namespace QuoteFlow.Api.Index
{
    public interface ISearcherFunction<T>
    {
        T Apply(IndexSearcher assetSearcher);
    }
}