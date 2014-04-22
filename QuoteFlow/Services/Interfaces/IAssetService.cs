using System.Collections.Generic;
using QuoteFlow.Models;
using QuoteFlow.Models.ViewModels;

namespace QuoteFlow.Services.Interfaces
{
    public interface IAssetService
    {
        /// <summary>
        /// Gets an asset and all of its relative properties such as manufacturer info and pricing.
        /// </summary>
        /// <param name="assetId">The Id of the asset to fetch.</param>
        /// <returns></returns>
        Asset GetAsset(int assetId);

        /// <summary>
        /// Return a list of all the assets for a given catalog along with the manufacturer info.
        /// </summary>
        /// <param name="catalogId">The catalog Id you wish to fetch from.</param>
        /// <returns></returns>
        IEnumerable<Asset> GetAssets(int catalogId);

        /// <summary>
        /// Return a list of all the assets for a given <see cref="Catalog"/>.
        /// </summary>
        /// <param name="catalog">A Catalog object which contains a populated Id field.</param>
        /// <returns></returns>
        IEnumerable<Asset> GetAssets(Catalog catalog);

        /// <summary>
        /// Creates an <see cref="Asset"/>.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="catalogId"></param>
        /// <param name="userId">The identifier of the <see cref="User"/> who is creating this asset.</param>
        /// <returns></returns>
        Asset CreateAsset(NewAssetModel model, int catalogId, int userId);

        /// <summary>
        /// Check if an asset exists.
        /// </summary>
        /// <param name="assetName">The asset name to search for</param>
        /// <param name="catalogId">The Id of the catalog to search from</param>
        /// <returns></returns>
        bool AssetExists(string assetName, int catalogId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cost"></param>
        /// <param name="markup"></param>
        /// <returns></returns>
        decimal CalculatePrice(decimal cost, decimal markup);
    }
}