using System.Collections.Generic;
using QuoteFlow.Api.Configuration;
using QuoteFlow.Api.Infrastructure.Services;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Models.ViewModels.Assets.Nav;

namespace QuoteFlow.Api.Asset.Nav
{
    /// <summary>
    /// Executes a search and returns a an asset table view model, stable search IDs, etc.
    /// </summary>
    public interface IAssetTableService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user">The user performing the search</param>
        /// <param name="filterId"></param>
        /// <param name="jql"></param>
        /// <param name="config"></param>
        /// <param name="isStableSearchFirstHit"></param>
        /// <returns></returns>
        IServiceOutcome<AssetTableViewModel> GetIssueTableFromFilterWithJql(User user, string filterId, string jql,
            IAssetTableServiceConfiguration config, bool isStableSearchFirstHit);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user">The user performing the search</param>
        /// <param name="filterId">the id of the filter used in the initial search for the list of <paramref name="ids"/></param>
        /// <param name="jql">The jql of the client state at the time of making this service call. Used for generating the correct sort JQL.</param>
        /// <param name="ids"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        AssetTableViewModel GetAssetTableFromAssetIds(User user, string filterId, string jql, List<int> ids,
            IAssetTableServiceConfiguration config);
    }
}