using QuoteFlow.Models;

namespace QuoteFlow.Services.Interfaces
{
    public interface IAssetPriceService
    {
        /// <summary>
        /// Gets the asset price for a particular asset.
        /// </summary>
        /// <param name="assetId">The Id of the asset to search for.</param>
        /// <returns>An AssetPrice object</returns>
        AssetPrice GetAssetPrice(int assetId);

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