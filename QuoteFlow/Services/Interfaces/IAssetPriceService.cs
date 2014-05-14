using System.Collections.Generic;
using QuoteFlow.Models;

namespace QuoteFlow.Services.Interfaces
{
    public interface IAssetPriceService
    {
        /// <summary>
        /// Gets the asset prices for a particular asset.
        /// </summary>
        /// <param name="assetId">The Id of the asset to search for.</param>
        /// <returns></returns>
        IEnumerable<AssetPrice> GetAssetPrices(int assetId);

        /// <summary>
        /// Inserts a new asset price object into the AssetPrices table
        /// </summary>
        /// <param name="assetPrice"></param>
        /// <returns></returns>
        AssetPrice InsertPrice(AssetPrice assetPrice);

        /// <summary>
        /// Updates an existing asset price record
        /// </summary>
        /// <param name="assetPrice">Modified asset price object</param>
        /// <returns>The updated asset price object</returns>
        AssetPrice UpdatePrice(AssetPrice assetPrice);
    }
}