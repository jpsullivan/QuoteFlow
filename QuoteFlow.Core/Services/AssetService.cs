using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Lucene.Net.Documents;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Models.ViewModels.Assets;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Asset;
using StackExchange.Profiling.Helpers.Dapper;
using DynamicParameters = Dapper.DynamicParameters;

namespace QuoteFlow.Core.Services
{
    public class AssetService : IAssetService
    {
        #region IoC

        public IAssetVarService AssetVarService { get; protected set; }
        public IFieldManager FieldManager { get; protected set; }
        private IManufacturerService ManufacturerService { get; set; }

        public AssetService() { }

        public AssetService(
            IAssetVarService assetVarService,
            IFieldManager fieldManager,
            IManufacturerService manufacturerService)
        {
            AssetVarService = assetVarService;
            FieldManager = fieldManager;
            ManufacturerService = manufacturerService;
        }

        #endregion

        /// <summary>
        /// Gets an asset and all of its relative properties such as manufacturer info and pricing.
        /// </summary>
        /// <param name="assetId">The Id of the asset to fetch.</param>
        /// <returns>An asset object</returns>
        public Api.Models.Asset GetAsset(int assetId)
        {
            var asset = Current.DB.Assets.Get(assetId);
            var catalog = Current.DB.Catalogs.Get(asset.CatalogId); // can't use CatalogService here due to cyclical deps
            var manufacturer = ManufacturerService.GetManufacturer(asset.ManufacturerId);
            var user = Current.DB.Users.Get(asset.CreatorId); // also can't use service due to cyclical deps
            var assetVars = AssetVarService.GetAssetVarsWithValues(assetId);
            var comments = GetAssetComments(assetId);

            asset.AssetVars = assetVars;
            asset.Catalog = catalog;
            asset.Creator = user;
            asset.Comments = comments;
            asset.Manufacturer = manufacturer;
            
            return asset;
        }

        /// <summary>
        /// Creates an <see cref="QuoteFlow.Core.Asset"/> object for an asset represented by the Lucene document.
        /// </summary>
        /// <param name="assetDocument">The Lucene document representing an asset.</param>
        /// <returns></returns>
        public IAsset GetAsset(Document assetDocument)
        {
            return new DocumentAsset(assetDocument, FieldManager, this, new CatalogService());
        }

        /// <summary>
        /// Return a list of all the assets for a given set of catalogs along with 
        /// their respective manufacturer info.
        /// </summary>
        /// <param name="catalogId"></param>
        /// <returns>A collection of <see cref="QuoteFlow.Core.Asset"/> records associated with one <see cref="Catalog"/></returns>
        public IEnumerable<Api.Models.Asset> GetAssets(int catalogId)
        {
            const string sql = @"select a.*, m.*, c.* from Assets a
                        join Manufacturers m on m.Id = a.ManufacturerId
                        join Catalogs c on c.Id = a.CatalogId
                        where a.CatalogId = @catalogId";

            // Multi-mapping won't work out of the box for this since we can't supply an
            // IEnumerable<AssetPrice>. Instead, we just manually map the required fields
            // below, supplying the properties with default values if null.
            var identityMap = new Dictionary<int, Api.Models.Asset>();
            var assets = Current.DB.Query<Api.Models.Asset, Manufacturer, Catalog, Api.Models.Asset>(sql, (a, m, c) =>
            {
                Api.Models.Asset master;
                if (!identityMap.TryGetValue(a.Id, out master)) {
                    identityMap[a.Id] = master = a;
                }

                var manufacturer = master.Manufacturer;
                if (manufacturer == null) {
                    master.Manufacturer = manufacturer = new Manufacturer();
                }
                master.Manufacturer = m;

                var catalog = master.Catalog;
                if (catalog == null) {
                    master.Catalog = catalog = new Catalog();
                }
                master.Catalog = c;

                return master;
            }, new { catalogId }).Distinct();

            return assets;
        }

        /// <summary>
        /// Return a list of all the assets for a given <see cref="Catalog"/>.
        /// </summary>
        /// <param name="catalog">A Catalog object which contains a populated Id field.</param>
        /// <returns>Collection of assets</returns>
        public IEnumerable<Api.Models.Asset> GetAssets(Catalog catalog)
        {
            return GetAssets(catalog.Id);
        }

        /// <summary>
        /// Creates an <see cref="QuoteFlow.Core.Asset"/>.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="catalogId"></param>
        /// <param name="userId">The identifier of the <see cref="User"/> who is creating this asset.</param>
        /// <returns>The newly created <see cref="QuoteFlow.Core.Asset"/></returns>
        public Api.Models.Asset CreateAsset(NewAssetModel model, int catalogId, int userId)
        {
            var asset = new Api.Models.Asset
            {
                Name = model.Name,
                Description = model.Description,
                Type = "Asset",
                CatalogId = catalogId,
                CreatorId = userId,
                CreationDate = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
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
                asset.CatalogId,
                asset.CreatorId,
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
        /// Creates an <see cref="QuoteFlow.Core.Asset"/>.
        /// </summary>
        /// <param name="asset">A pre-built <see cref="QuoteFlow.Core.Asset"/>.</param>
        /// <param name="userId">The identifier of the <see cref="User"/> who is creating this asset.</param>
        /// <returns></returns>
        public Api.Models.Asset CreateAsset(Api.Models.Asset asset, int userId)
        {
            if (asset == null) {
                throw new ArgumentNullException("asset");
            }

            if (userId == 0) {
                throw new ArgumentException("User Id must be greater than zero.", "userId");
            }

            // Fill in any fields that one shouldn't be concerned with elsewhere
            asset.Type = "Asset";

            if (asset.CreationDate.Equals(DateTime.MinValue)) {
                asset.CreationDate = DateTime.UtcNow;
            }

            if (asset.LastUpdated.Equals(DateTime.MinValue)) {
                asset.LastUpdated = DateTime.UtcNow;
            }

            int? insert = Current.DB.Assets.Insert(new
            {
                asset.Name,
                asset.SKU,
                asset.Type,
                asset.Description,
                asset.Cost,
                asset.Markup,
                asset.ManufacturerId,
                asset.CatalogId,
                asset.CreatorId,
                asset.CreationDate,
                asset.LastUpdated
            });

            if (insert != null) {
                asset.Id = insert.Value;
            }

            return asset;
        }

        /// <summary>
        /// Upadtes an assets details based on a <see cref="Snapshotter"/> diff.
        /// </summary>
        /// <param name="assetId">The asset id to update.</param>
        /// <param name="diff">The <see cref="Snapshotter"/> diff.</param>
        public void UpdateAsset(int assetId, DynamicParameters diff)
        {
            if (assetId == 0) {
                throw new ArgumentException("Asset ID must be greater than zero.", "assetId");
            }
            
            Current.DB.Assets.Update(assetId, diff);
        }

        /// <summary>
        /// Retrieves a collection of <see cref="AssetComment"/>s relative to
        /// the specific <see cref="assetId"/>.
        /// </summary>
        /// <param name="assetId">The id of the assset whose comments are being fetched.</param>
        /// <returns></returns>
        public IEnumerable<AssetComment> GetAssetComments(int assetId)
        {
            const string sql = @"select ac.*, u.* from AssetComments ac
                        join Users u on u.Id = ac.CreatorId
                        where ac.AssetId = @assetId";

            var comments = Current.DB.Query<AssetComment, User, AssetComment>(sql, (comment, user) => {
                comment.Creator = user;
                return comment;
            }, new {assetId});

            return comments;

//            const string sql = "select * from AssetComments where AssetId = @assetId";
//            return Current.DB.Query<AssetComment>(sql, new {assetId});
        }

        /// <summary>
        /// Creates a new <see cref="AssetComment"/>.
        /// </summary>
        /// <param name="comment">The comment itself.</param>
        /// <param name="assetId">The asset which this comment is for.</param>
        /// <param name="userId">The user who wrote the comment.</param>
        public void AddAssetComment(string comment, int assetId, int userId)
        {
            // don't do anything if the comment itself is empty
            if (!comment.HasValue()) return;

            var ac = new AssetComment(comment, assetId, userId);
            Current.DB.AssetComments.Insert(ac);
        }

        /// <summary>
        /// Check if an asset exists.
        /// </summary>
        /// <param name="assetName">The asset name to search for</param>
        /// <param name="catalogId">The Id of the <see cref="Catalog"/> to search from</param>
        /// <returns>True if asset exists, false if not.</returns>
        public bool AssetExists(string assetName, int catalogId)
        {
            const string sql = "select * from Assets where Name = @assetName AND CatalogId = @catalogId";
            var asset = Current.DB.Query<Api.Models.Asset>(sql, new
            {
                assetName, 
                catalogId
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
        /// <param name="catalogId">The Id of the <see cref="Catalog"/> to search from.</param>
        /// <param name="asset"></param>
        /// <returns></returns>
        public bool AssetExists(string name, int manufacturerId, string description, string sku, int catalogId, out Api.Models.Asset asset)
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
            builder.Where("CatalogId = @catalogId");

            var foundAsset = Current.DB.Query<Api.Models.Asset>(tmpl.RawSql, new
            {
                name, description, sku, manufacturerId, catalogId
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
        /// <param name="catalogId">The Id of the <see cref="Catalog"/> to search from.</param>
        /// <param name="asset"></param>
        /// <returns></returns>
        public bool AssetExists(string sku, int manufacturerId, int catalogId, out Api.Models.Asset asset)
        {
            var builder = new SqlBuilder();
            SqlBuilder.Template tmpl = builder.AddTemplate(@"
                SELECT * FROM Assets
                /**where**/"
            );
            
            builder.Where("SKU = @sku");
            builder.Where("ManufacturerId = @manufacturerId");
            builder.Where("CatalogId = @catalogId");

            var foundAsset = Current.DB.Query<Api.Models.Asset>(tmpl.RawSql, new
            {
                sku,
                manufacturerId, 
                catalogId
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

        /// <summary>
        /// Returns a set of catalog ID / manufacturer combinations that given asset IDs cover.
        /// </summary>
        /// <param name="assetIds">Set of asset IDs.</param>
        /// <returns>Catalog ID / Manufacturer pairs</returns>
        public ISet<KeyValuePair<int, string>> GetCatalogManufacturerPairsByIds(ISet<int> assetIds)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check existance of assets for hte given set of IDs.
        /// </summary>
        /// <param name="assetIds">Set of asset IDs</param>
        /// <returns>Set of IDs that don't represent an asset.</returns>
        public ISet<int> GetIdsOfMissingAssets(ISet<int> assetIds)
        {
            throw new NotImplementedException();
        }
    }
}