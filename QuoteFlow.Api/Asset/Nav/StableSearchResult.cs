using System.Collections.Generic;
using QuoteFlow.Api.Infrastructure.Services;
using QuoteFlow.Api.Models.ViewModels.Assets.Nav;

namespace QuoteFlow.Api.Asset.Nav
{
    /// <summary>
    /// Response from the StableSearchService and asset table API requests.
    /// </summary>
    public class StableSearchResult
    {
        public AssetTable AssetTable { get; set; }
        public IEnumerable<string> Warnings { get; set; }

        public StableSearchResult()
        {
        }

        public StableSearchResult(IServiceOutcome<AssetTableViewModel> assetTableOutcome)
        {
            if (assetTableOutcome.IsValid())
            {
                var value = assetTableOutcome.ReturnedValue;
                AssetTable = value.AssetTable;
                Warnings = value.Warnings;
            }
            else
            {
                AssetTable = null;
                Warnings = null;
            }
        }
    }
}