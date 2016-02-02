using System.Collections.Generic;
using QuoteFlow.Api.Configuration;
using QuoteFlow.Api.Infrastructure.Services;

namespace QuoteFlow.Api.Asset.Nav
{
    /// <summary>
    /// Returns details about stable searches.
    /// </summary>
    public interface IStableSearchService
    {
        IServiceOutcome<StableSearchResult> GetAssetTableFromAssetIds(string filterId, string jql, List<int?> ids, IAssetTableServiceConfiguration config);
    }
}