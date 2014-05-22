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
        /// <param name="catalogIds"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        IEnumerable<Asset> GetAssets(IEnumerable<int> catalogIds, int organizationId);

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
        /// <param name="organizationId"></param>
        /// <param name="catalogId"></param>
        /// <param name="userId">The identifier of the <see cref="User"/> who is creating this asset.</param>
        /// <returns></returns>
        Asset CreateAsset(NewAssetModel model, int organizationId, int catalogId, int userId);

        /// <summary>
        /// Creates an <see cref="Asset"/>.
        /// </summary>
        /// <param name="asset">A pre-built <see cref="Asset"/>.</param>
        /// <param name="userId">The identifier of the <see cref="User"/> who is creating this asset.</param>
        /// <returns></returns>
        Asset CreateAsset(Asset asset, int userId);

        /// <summary>
        /// Creates an <see cref="Asset"/>.
        /// </summary>
        /// <param name="asset">A pre-built <see cref="Asset"/>.</param>
        /// <param name="price"></param>
        /// <param name="userId">The identifier of the <see cref="User"/> who is creating this asset.</param>
        /// <returns></returns>
        Asset CreateAsset(Asset asset, AssetPrice price, int userId);

        /// <summary>
        /// Check if an asset exists.
        /// </summary>
        /// <param name="assetName">The asset name to search for</param>
        /// <param name="organizationId">The Id of the catalog to search from</param>
        /// <returns></returns>
        bool AssetExists(string assetName, int organizationId);

        /// <summary>
        /// Check if an asset (truly) exists. Checks with a much more 
        /// specific set of paramters to match on.
        /// </summary>
        /// <param name="name">The asset name.</param>
        /// <param name="manufacturerId">The identifier for the <see cref="Manufacturer"/>.</param>
        /// <param name="description">The asset description.</param>
        /// <param name="sku">The asset SKU.</param>
        /// <param name="organizationId">The Id of the organization to search from.</param>
        /// <param name="asset"></param>
        /// <returns></returns>
        bool AssetExists(string name, int manufacturerId, string description, string sku, int organizationId, out Asset asset);

        /// <summary>
        /// Check if an asset exists based on its SKU.
        /// </summary>
        /// <param name="sku">The asset SKU (part number)</param>
        /// <param name="manufacturerId">The identifier for the <see cref="Manufacturer"/>.</param>
        /// <param name="organizationId">The Id of the organization to search from.</param>
        /// <param name="asset"></param>
        /// <returns></returns>
        bool AssetExists(string sku, int manufacturerId, int organizationId, out Asset asset);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cost"></param>
        /// <param name="markup"></param>
        /// <returns></returns>
        decimal CalculatePrice(decimal cost, decimal markup);

        /// <summary>
        /// Returns whether or not the specified asset name exceeds the
        /// maximum length of 250 characters (which is still way too high, people).
        /// </summary>
        /// <param name="name">The asset name</param>
        /// <returns></returns>
        bool AssetNameExceedsMaximumLength(string name);
    }
}