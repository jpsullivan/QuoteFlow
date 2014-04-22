using System;
using System.Linq;
using Dapper;
using QuoteFlow.Models;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Services
{
    public class AssetPriceService : IAssetPriceService
    {
        /// <summary>
        /// Gets the asset price for a particular asset.
        /// </summary>
        /// <param name="assetId">The Id of the asset to search for.</param>
        /// <returns>An AssetPrice object</returns>
        public AssetPrice GetAssetPrice(int assetId)
        {
            var assetPrice = Current.DB.Query<AssetPrice>("select * from AssetPrices where AssetId = @assetId", new
            {
                assetId
            }).Single();
            return assetPrice;
        }

        /// <summary>
        /// Inserts a new asset price object into the AssetPrices table
        /// </summary>
        /// <param name="assetPrice"></param>
        /// <returns></returns>
        public AssetPrice InsertPrice(AssetPrice assetPrice)
        {
            var nowUtc = DateTime.UtcNow;
            assetPrice.CreationDate = nowUtc;
            assetPrice.LastUpdated = nowUtc;
            int? insert = Current.DB.AssetPrices.Insert(assetPrice);

            if (insert != null)
            {
                assetPrice.Id = insert.Value;
            }

            return assetPrice;
        }

        /// <summary>
        /// Updates an existing asset price record
        /// </summary>
        /// <param name="assetPrice">Modified asset price object</param>
        /// <returns>The updated asset price object</returns>
        public AssetPrice UpdatePrice(AssetPrice assetPrice)
        {
            AssetPrice original = Current.DB.AssetPrices.Get(assetPrice.Id);

            // track which fields change on the object
            var s = Snapshotter.Start(original);
            original.Cost = assetPrice.Cost;
            original.Markup = assetPrice.Markup;
            original.Price = assetPrice.Price;
            original.LastUpdated = DateTime.UtcNow;

            var diff = s.Diff();
            if (diff.ParameterNames.Any())
            {
                Current.DB.AssetPrices.Update(assetPrice.Id, diff);
            }

            // Re-fetching the asset probably isn't necessary... Look into stripping this out
            var refetched = Current.DB.AssetPrices.Get(assetPrice.Id);

            return refetched;
        }
    }
}