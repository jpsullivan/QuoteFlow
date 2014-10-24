using QuoteFlow.Infrastructure.Enumerables;
using QuoteFlow.Models.Search.Jql;

namespace QuoteFlow.Services.Interfaces
{
    public interface IAssetSearchService
    {
        SearchResults Search(ListWithDuplicates paramMap, long paramLong);

        SearchResults SearchWithJql(string paramString, long paramLong);
    }
}