using QuoteFlow.Models;
using QuoteFlow.Models.Search.Jql.Query;
using Wintellect.PowerCollections;

namespace QuoteFlow.Services.Interfaces
{
    public interface IAssetSearchService
    {
        QuerySearchResults Search(User user, MultiDictionary<string, string[]> paramMap, long filterId);

        QuerySearchResults SearchWithJql(User user, string paramString, long filterId);
    }
}