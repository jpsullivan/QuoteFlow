using System.Collections.Generic;
using Dapper;
using Lucene.Net.Documents;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Models.ViewModels.Assets;

namespace QuoteFlow.Api.Services
{
    public interface IAssetService
    {
        /// <summary>
        /// Gets an asset and all of its relative properties such as manufacturer info and pricing.
        /// </summary>
        /// <param name="assetId">The Id of the asset to fetch.</param>
        /// <returns></returns>
        Api.Models.Asset GetAsset(int assetId);

        /// <summary>
        /// Creates an <see cref="Asset"/> object for an asset represented by the Lucene document.
        /// </summary>
        /// <param name="assetDocument">The Lucene document representing an asset.</param>
        /// <returns></returns>
        IAsset GetAsset(Document assetDocument);

        /// <summary>
        /// Return a list of all the assets for a given catalog along with the manufacturer info.
        /// </summary>
        /// <param name="catalogId"></param>
        /// <returns></returns>
        IEnumerable<Api.Models.Asset> GetAssets(int catalogId);

        /// <summary>
        /// Return a list of all the assets for a given <see cref="Catalog"/>.
        /// </summary>
        /// <param name="catalog">A Catalog object which contains a populated Id field.</param>
        /// <returns></returns>
        IEnumerable<Api.Models.Asset> GetAssets(Catalog catalog);

        /// <summary>
        /// Creates an <see cref="Asset"/>.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="catalogId"></param>
        /// <param name="userId">The identifier of the <see cref="User"/> who is creating this asset.</param>
        /// <returns></returns>
        Api.Models.Asset CreateAsset(NewAssetModel model, int catalogId, int userId);

        /// <summary>
        /// Creates an <see cref="Asset"/>.
        /// </summary>
        /// <param name="asset">A pre-built <see cref="Asset"/>.</param>
        /// <param name="userId">The identifier of the <see cref="User"/> who is creating this asset.</param>
        /// <returns></returns>
        Api.Models.Asset CreateAsset(Api.Models.Asset asset, int userId);

        /// <summary>
        /// Updates an assets details based on a <see cref="Snapshotter"/> diff.
        /// </summary>
        /// <param name="assetId">The asset id to update.</param>
        /// <param name="diff">The <see cref="Snapshotter"/> diff.</param>
        void UpdateAsset(int assetId, DynamicParameters diff);

        /// <summary>
        /// Retrieves a collection of <see cref="AssetComment"/>s relative to
        /// the specific <see cref="assetId"/>.
        /// </summary>
        /// <param name="assetId">The id of the assset whose comments are being fetched.</param>
        /// <returns></returns>
        IEnumerable<AssetComment> GetAssetComments(int assetId);

        /// <summary>
        /// Creates a new <see cref="AssetComment"/>.
        /// </summary>
        /// <param name="comment">The comment itself.</param>
        /// <param name="assetId">The asset which this comment is for.</param>
        /// <param name="userId">The user who wrote the comment.</param>
        void AddAssetComment(string comment, int assetId, int userId);

        /// <summary>
        /// Check if an asset exists.
        /// </summary>
        /// <param name="assetName">The asset name to search for</param>
        /// <param name="catalogId">The Id of the catalog to search from</param>
        /// <returns></returns>
        bool AssetExists(string assetName, int catalogId);

        /// <summary>
        /// Check if an asset (truly) exists. Checks with a much more 
        /// specific set of paramters to match on.
        /// </summary>
        /// <param name="name">The asset name.</param>
        /// <param name="manufacturerId">The identifier for the <see cref="Manufacturer"/>.</param>
        /// <param name="description">The asset description.</param>
        /// <param name="sku">The asset SKU.</param>
        /// <param name="catalogId">The Id of the organization to search from.</param>
        /// <param name="asset"></param>
        /// <returns></returns>
        bool AssetExists(string name, int manufacturerId, string description, string sku, int catalogId, out Models.Asset asset);

        /// <summary>
        /// Check if an asset exists based on its SKU.
        /// </summary>
        /// <param name="sku">The asset SKU (part number)</param>
        /// <param name="manufacturerId">The identifier for the <see cref="Manufacturer"/>.</param>
        /// <param name="catalogId">The Id of the organization to search from.</param>
        /// <param name="asset"></param>
        /// <returns></returns>
        bool AssetExists(string sku, int manufacturerId, int catalogId, out Models.Asset asset);

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

        /// <summary>
        /// Returns a set of catalog ID / manufacturer combinations that given asset IDs cover.
        /// </summary>
        /// <param name="assetIds">Set of asset IDs.</param>
        /// <returns>Catalog ID / Manufacturer pairs</returns>
        ISet<KeyValuePair<int, string>> GetCatalogManufacturerPairsByIds(ISet<int> assetIds);

        /// <summary>
        /// Check existance of assets for hte given set of IDs.
        /// </summary>
        /// <param name="assetIds">Set of asset IDs</param>
        /// <returns>Set of IDs that don't represent an asset.</returns>
        ISet<int> GetIdsOfMissingAssets(ISet<int> assetIds);
    }
}