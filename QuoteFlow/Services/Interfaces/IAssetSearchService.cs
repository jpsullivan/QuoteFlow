using QuoteFlow.Infrastructure.Enumerables;
using QuoteFlow.Models.Search.Jql;

namespace QuoteFlow.Services.Interfaces
{
    public interface IAssetSearchService
    {
        SearchResults Search(ListWithDuplicates paramMap, long filterId);

        SearchResults SearchWithJql(string paramString, long filterId);
    }
}