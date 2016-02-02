using System.Collections.Generic;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Jql.Util
{
    /// <summary>
    /// Some helper asset lookup functions for QuoteFlow.
    /// </summary>
    public interface IJqlAssetSupport
    {
        /// <summary>
		/// Get the asset given its id if the passed user can see it. A null will be returned if the asset id is
		/// not within QuoteFlow or if the user does not have permission to see the asset.
		/// </summary>
		/// <param name="id">The id of the asset to retreieve. A null key is assumed not to exist within QuoteFlow.</param>
		/// <param name="user">The user who must have permission to see the asset. </param>
		/// <returns>
		/// The asset identified by the passed id if it can be seen by the passed user. 
		/// A null value will be returned if the asset does not exist or the user cannot see the asset.
		/// </returns>
		IAsset GetAsset(int id, User user);

		/// <summary>
		/// Get the asset given its id. A null will be returned if the asset is not within QuoteFlow.
		/// </summary>
		/// <param name="id">The id of the asset to retrieve.</param>
		/// <returns>The asset identified by the passed id. A null value will be returned if the asset does not exist.</returns>
		IAsset GetAsset(int id);

        /// <summary>
		/// Returns a set of project ID / asset type combinations that given asset IDs cover.
		/// </summary>
		/// <param name="assetIds"> Set of asset IDs</param>
		/// <returns>Catalog ID / Manufacturer Pairs</returns>
		ISet<KeyValuePair<int, string>> GetCatalogManufacturerPairsByIds(ISet<int> assetIds);

        /// <summary>
        /// Check existence of assets for the given set of IDs.
        /// </summary>
        /// <param name="assetIds">Set of asset IDs.</param>
        /// <returns>Set of IDs that don't represent an asset.</returns>
        ISet<int> GetIdsOfMissingAssets(ISet<int> assetIds);

        /// <summary>
        /// Check existance of assets for the given set of SKU's.
        /// </summary>
        /// <param name="assetSkus"></param>
        /// <returns></returns>
        ISet<string> GetSkusOfMissingAssets(ISet<string> assetSkus);
    }
}