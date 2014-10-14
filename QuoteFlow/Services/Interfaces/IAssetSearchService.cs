using System.Collections.Generic;
using QuoteFlow.Models.Search.Jql;

namespace QuoteFlow.Services.Interfaces
{
    public interface IAssetSearchService
    {
        SearchResults Search(Dictionary<string, string[]> paramMap, long paramLong);

        SearchResults SearchWithJql(string paramString, long paramLong);
    }
}