using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Models;
using Wintellect.PowerCollections;

namespace QuoteFlow.Api.Services
{
    public interface IAssetSearchService
    {
        QuerySearchResults Search(User user, MultiDictionary<string, string[]> paramMap, long filterId);

        QuerySearchResults SearchWithJql(User user, string paramString, long filterId);
    }
}