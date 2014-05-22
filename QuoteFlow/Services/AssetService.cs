using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Models;
using QuoteFlow.Models.ViewModels;
using QuoteFlow.Services.Interfaces;
using StackExchange.Profiling.Helpers.Dapper;

namespace QuoteFlow.Services
{
    public class AssetService : IAssetService
    {
        #region IoC

        private IAssetPriceService AssetPriceService { get; set; }
        private IManufacturerService ManufacturerService { get; set; }

        public AssetService() { }

        public AssetService(IAssetPriceService assetPriceService, IManufacturerService manufacturerService)
        {
            AssetPriceService = assetPriceService;
            ManufacturerService = manufacturerService;
        }

        #endregion

        /// <summary>
        /// Gets an asset and all of its relative properties such as manufacturer info and pricing.
        /// </summary>
        /// <param name="assetId">The Id of the asset to fetch.</param>
        /// <returns>An asset object</returns>
        public Asset GetAsset(int assetId)
        {
            var asset = Current.DB.Assets.Get(assetId);
            var manufacturer = ManufacturerService.GetManufacturer(asset.ManufacturerId);
            var pricing = AssetPriceService.GetAssetPrices(assetId);

            asset.Manufacturer = manufacturer;
            asset.Prices = pricing;

            return asset;
        }

        /// <summary>
        /// Return a list of all the assets for a given set of catalogs along with 
        /// their respective manufacturer info.
        /// </summary>
        /// <param name="catalogIds"></param>
        /// <param name="organizationId"></param>
        /// <returns>A collection of <see cref="Asset"/> records associated with one <see cref="Catalog"/></returns>
        public IEnumerable<Asset> GetAssets(IEnumerable<int> catalogIds, int organizationId)
        {
            const string sql = @"select a.*, m.*, ap.* from Assets a
                        join Manufacturers m on m.Id = a.ManufacturerId
                        join AssetPrices ap on ap.AssetId = a.Id
                        where a.OrganizationId = @organizationId and ap.CatalogId in @catalogIds";

            // Multi-mapping won't work out of the box for this since we can't supply an
            // IEnumerable<AssetPrice>. Instead, we just manually map the required fields
            // below, supplying the properties with default values if null.
            var identityMap = new Dictionary<int, Asset>();
            var assets = Current.DB.Query<Asset, Manufacturer, AssetPrice, Asset>(sql, (a, m, ap) => {
                Asset master;
                if (!identityMap.TryGetValue(a.Id, out master)) {
                    identityMap[a.Id] = master = a;
                }

                var manufacturer = master.Manufacturer;
                if (manufacturer == null) {
                    master.Manufacturer = manufacturer = new Manufacturer();
                }
                master.Manufacturer = m;

                var prices = (List<AssetPrice>) master.Prices;
                if (prices == null) {
                    master.Prices = prices = new List<AssetPrice>();
                }
                prices.Add(ap);

                return master;
            }, new { catalogIds, organizationId }).Distinct();

            return assets;
        }

        /// <summary>
        /// Return a list of all the assets for a given <see cref="Catalog"/>.
        /// </summary>
        /// <param name="catalog">A Catalog object which contains a populated Id field.</param>
        /// <returns>Collection of assets</returns>
        public IEnumerable<Asset> GetAssets(Catalog catalog)
        {
            var catalogIds = new List<int> {catalog.Id};
            return GetAssets(catalogIds, catalog.OrganizationId);
        }

        /// <summary>
        /// Creates an <see cref="Asset"/>.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="organizationId"></param>
        /// <param name="catalogId"></param>
        /// <param name="userId">The identifier of the <see cref="User"/> who is creating this asset.</param>
        /// <returns>The newly created <see cref="Asset"/></returns>
        public Asset CreateAsset(NewAssetModel model, int organizationId, int catalogId, int userId)
        {
            var asset = new Asset
            {
                Name = model.Name,
                Description = model.Description,
                Type = "Asset",
                CreatorId = userId,
                CreationDate = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                OrganizationId = organizationId
            };
            var assetPrice = new AssetPrice
            {
                Cost = model.Cost,
                Markup = model.Markup,
                Price = CalculatePrice(model.Cost, model.Markup),
                CatalogId = catalogId
            };

            // Fetch the manufacturer id
            Manufacturer manufacturer = ManufacturerService.GetManufacturer(model.Manufacturer, true);
            asset.ManufacturerId = manufacturer.Id;

            int? insert = Current.DB.Assets.Insert(new
            {
                asset.Name,
                asset.Description,
                asset.Type,
                asset.ManufacturerId,
                asset.CreatorId,
                asset.OrganizationId,
                asset.CreationDate,
                asset.LastUpdated
            });

            if (insert != null)
            {
                asset.Id = insert.Value;
            }

            assetPrice.AssetId = asset.Id;

            // Insert the new asset price and set it to the asset
            AssetPriceService.InsertPrice(assetPrice);

            asset.Prices = AssetPriceService.GetAssetPrices(asset.Id);
            return asset;
        }

        /// <summary>
        /// Creates an <see cref="Asset"/>.
        /// </summary>
        /// <param name="asset">A pre-built <see cref="Asset"/>.</param>
        /// <param name="userId">The identifier of the <see cref="User"/> who is creating this asset.</param>
        /// <returns></returns>
        public Asset CreateAsset(Asset asset, int userId)
        {
            if (asset == null) {
                throw new ArgumentNullException("asset");
            }

            if (userId == 0) {
                throw new ArgumentException("User Id must be greater than zero.", "userId");
            }

            // Fill in any fields that one shouldn't be concerned with elsewhere
            asset.Type = "Asset";

            if (asset.CreationDate.Equals(DateTime.MinValue))
            {
                asset.CreationDate = DateTime.UtcNow;
            }

            if (asset.LastUpdated.Equals(DateTime.MinValue))
            {
                asset.LastUpdated = DateTime.UtcNow;
            }

            int? insert = Current.DB.Assets.Insert(new
            {
                asset.Name,
                asset.SKU,
                asset.Description,
                asset.Type,
                asset.ManufacturerId,
                asset.CreatorId,
                asset.OrganizationId,
                asset.CreationDate,
                asset.LastUpdated
            });

            if (insert != null)
            {
                asset.Id = insert.Value;
            }

            return asset;
        }

        /// <summary>
        /// Creates an <see cref="Asset"/>.
        /// </summary>
        /// <param name="asset">A pre-built <see cref="Asset"/>.</param>
        /// <param name="price"></param>
        /// <param name="userId">The identifier of the <see cref="User"/> who is creating this asset.</param>
        /// <returns></returns>
        public Asset CreateAsset(Asset asset, AssetPrice price, int userId)
        {
            if (asset == null) {
                throw new ArgumentNullException("asset");
            }

            if (price == null) {
                throw new ArgumentNullException("price");
            }

            if (userId == 0) {
                throw new ArgumentException("User Id must be greater than zero.", "userId");
            }

            asset = CreateAsset(asset, userId);

            price.AssetId = asset.Id;

            // Insert the new asset price and set it to the asset
            AssetPriceService.InsertPrice(price);

            asset.Prices = AssetPriceService.GetAssetPrices(asset.Id);
            return asset;
        }

        /// <summary>
        /// Check if an asset exists.
        /// </summary>
        /// <param name="assetName">The asset name to search for</param>
        /// <param name="organizationId">The Id of the organization to search from</param>
        /// <returns>True if asset exists, false if not.</returns>
        public bool AssetExists(string assetName, int organizationId)
        {
            const string sql = "select * from Assets where Name = @assetName AND OrganizationId = @organizationId";
            var asset = Current.DB.Query<Asset>(sql, new
            {
                assetName,
                catalogId = organizationId
            }).FirstOrDefault();

            return asset != null;
        }

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
        public bool AssetExists(string name, int manufacturerId, string description, string sku, int organizationId, out Asset asset)
        {
            var builder = new SqlBuilder();
            SqlBuilder.Template tmpl = builder.AddTemplate(@"
                SELECT * FROM Assets
                /**where**/"
            );

            builder.Where("Name = @name");
            builder.Where("Description = @description");
            builder.Where("SKU = @sku");
            builder.Where("ManufacturerId = @manufacturerId");
            builder.Where("OrganizationId = @organizationId");

            var foundAsset = Current.DB.Query<Asset>(tmpl.RawSql, new
            {
                name, description, sku, manufacturerId, catalogId = organizationId
            }).FirstOrDefault();

            if (foundAsset == null) {
                asset = null;
                return false;
            }

            asset = GetAsset(foundAsset.Id);
            return true;
        }

        /// <summary>
        /// Check if an asset exists based on its SKU.
        /// </summary>
        /// <param name="sku">The asset SKU (part number)</param>
        /// <param name="manufacturerId">The identifier for the <see cref="Manufacturer"/>.</param>
        /// <param name="organizationId">The Id of the organization to search from.</param>
        /// <param name="asset"></param>
        /// <returns></returns>
        public bool AssetExists(string sku, int manufacturerId, int organizationId, out Asset asset)
        {
            var builder = new SqlBuilder();
            SqlBuilder.Template tmpl = builder.AddTemplate(@"
                SELECT * FROM Assets
                /**where**/"
            );
            
            builder.Where("SKU = @sku");
            builder.Where("ManufacturerId = @manufacturerId");
            builder.Where("OrganizationId = @organizationId");

            var foundAsset = Current.DB.Query<Asset>(tmpl.RawSql, new
            {
                sku,
                manufacturerId, 
                organizationId
            }).FirstOrDefault();

            if (foundAsset == null) {
                asset = null;
                return false;
            }

            asset = GetAsset(foundAsset.Id);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cost"></param>
        /// <param name="markup"></param>
        /// <returns></returns>
        public decimal CalculatePrice(decimal cost, decimal markup)
        {
            var percent = (markup * 100) / cost;
            var margin = (cost * (percent / 100));
            var price = (cost + margin);
            return price;
        }

        /// <summary>
        /// Returns whether or not the specified asset name exceeds the
        /// maximum length of 250 characters (which is still way too high, people).
        /// </summary>
        /// <param name="name">The asset name</param>
        /// <returns></returns>
        public bool AssetNameExceedsMaximumLength(string name)
        {
            return name.Length > 250;
        }
    }
}