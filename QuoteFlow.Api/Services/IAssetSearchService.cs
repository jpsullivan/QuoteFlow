using QuoteFlow.Api.Infrastructure.Services;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Models;
using Wintellect.PowerCollections;

namespace QuoteFlow.Api.Services
{
    public interface IAssetSearchService
    {
        IServiceOutcome<QuerySearchResults> Search(User user, MultiDictionary<string, string[]> paramMap, long filterId);

        IServiceOutcome<QuerySearchResults> SearchWithJql(User user, string jqlContext, long filterId);

        IServiceOutcome<string> GetEditHtml(string searcherId, string jqlContext);

        Searchers GetSearchers(string jqlContext);
    }
}