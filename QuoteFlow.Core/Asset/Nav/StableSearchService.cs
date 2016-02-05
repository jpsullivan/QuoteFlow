using System.Collections.Generic;
using QuoteFlow.Api.Asset.Nav;
using QuoteFlow.Api.Configuration;
using QuoteFlow.Api.Infrastructure.Services;
using QuoteFlow.Api.Util;
using QuoteFlow.Core.Infrastructure.Services;

namespace QuoteFlow.Core.Asset.Nav
{
    /// <summary>
    /// Returns details about stable searches.
    /// </summary>
    public class StableSearchService : IStableSearchService
    {
        public IAssetTableService AssetTableService { get; protected set; }

        public StableSearchService(IAssetTableService assetTableService)
        {
            AssetTableService = assetTableService;
        }

        public IServiceOutcome<StableSearchResult> GetAssetTableFromAssetIds(string filterId, string jql, List<int?> ids, IAssetTableServiceConfiguration config)
        {
            var assetTableOutcome = AssetTableService.GetAssetTableFromAssetIds(null, filterId, jql, ids, config);
            var errorCollection = new SimpleErrorCollection();
            var result = new StableSearchResult(assetTableOutcome);
            if (assetTableOutcome.ErrorCollection != null)
            {
                errorCollection.AddErrorCollection(assetTableOutcome.ErrorCollection);
            }

            return new ServiceOutcome<StableSearchResult>(errorCollection, result);
        }
    }
}