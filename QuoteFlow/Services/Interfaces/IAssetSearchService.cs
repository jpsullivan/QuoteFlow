using QuoteFlow.Models.Search.Jql.Query;
using Wintellect.PowerCollections;

namespace QuoteFlow.Services.Interfaces
{
    public interface IAssetSearchService
    {
        QuerySearchResults Search(MultiDictionary<string, string[]> paramMap, long filterId);

        QuerySearchResults SearchWithJql(string paramString, long filterId);
    }
}