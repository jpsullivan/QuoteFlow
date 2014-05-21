using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Infrastructure.Extensions;
using QuoteFlow.Models;
using QuoteFlow.Models.CatalogImport;
using QuoteFlow.Models.ViewModels;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Services
{
    public class CatalogService : ICatalogService
    {
        #region IoC

        private IAssetPriceService AssetPriceService { get; set; }
        private IAssetService AssetService { get; set; }
        private IAssetVarService AssetVarService { get; set; }
        private ICatalogImportSummaryRecordsService CatalogSummaryService { get; set; }
        private IManufacturerService ManufacturerService { get; set; }

        public CatalogService() { }

        public CatalogService(IAssetPriceService assetPriceService, IAssetService assetService, 
            ICatalogImportSummaryRecordsService catalogSummaryService, IAssetVarService assetVarService, 
            IManufacturerService manufacturerService)
        {
            AssetPriceService = assetPriceService;
            AssetService = assetService;
            AssetVarService = assetVarService;
            CatalogSummaryService = catalogSummaryService;
            ManufacturerService = manufacturerService;
        }

        #endregion

        /// <summary>
        /// Fetch a <see cref="Catalog"/> based on its id.
        /// </summary>
        /// <param name="catalogId">The <see cref="Catalog"/> identifier.</param>
        /// <returns></returns>
        public Catalog GetCatalog(int catalogId)
        {
            return Current.DB.Catalogs.Get(catalogId);
        }

        /// <summary>
        /// Fetches a <see cref="Catalog"/> based on its name.
        /// </summary>
        /// <param name="catalogName">The <see cref="Catalog"/> name</param>
        /// <returns></returns>
        public Catalog GetCatalog(string catalogName)
        {
            const string sql = "select * from Catalogs where Name = @catalogName";
            return Current.DB.Query<Catalog>(sql, new {catalogName}).FirstOrDefault();
        }

        /// <summary>
        /// Fetches all of the catalogs within a given organization.
        /// </summary>
        /// <param name="organizationId">The organization id</param>
        /// <returns></returns>
        public IEnumerable<Catalog> GetCatalogs(int organizationId)
        {
            const string sql = "select * from catalogs where OrganizationId = @organizationId";
            return Current.DB.Query<Catalog>(sql, new {organizationId});
        }

        /// <summary>
        /// Adds a new catalog from the database based on ViewModel data from the new catalog form
        /// </summary>
        /// <param name="model">NewCatalogModel ViewModel</param>
        /// <param name="userId">The identifier of the <see cref="User"/> who is creating this catalog.</param>
        /// <returns></returns>
        public Catalog CreateCatalog(NewCatalogModel model, int userId)
        {
            if (!model.Expirable)
            {
                model.ExpirationDate = null;
            }

            var catalog = new Catalog
            {
                CreationDate = DateTime.UtcNow,
                CreatorId = userId,
                Enabled = true,
                ExpirationDate = model.ExpirationDate,
                LastUpdated = DateTime.UtcNow,
                Name = model.Name,
                Description = model.Description,
                OrganizationId = model.Organization.Id
            };

            var insert = Current.DB.Catalogs.Insert(new
            {
                catalog.Name,
                catalog.Description,
                catalog.OrganizationId,
                catalog.CreatorId,
                catalog.ExpirationDate,
                catalog.CreationDate,
                catalog.LastUpdated,
                catalog.Enabled
            });

            if (insert != null)
            {
                catalog.Id = insert.Value;
            }

            return catalog;
        }

        /// <summary>
        /// Check to see if any catalog's with the supplied name currently exists within
        /// the supplied organization id.
        /// </summary>
        /// <param name="catalogName"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public bool CatalogNameExists(string catalogName, int organizationId)
        {
            var catalog =
                Current.DB.Query<Catalog>(
                    "select * from Catalogs where Name = @catalogName AND OrganizationId = @organizationId", new
                    {
                        catalogName,
                        organizationId
                    }).FirstOrDefault();

            return catalog != null;
        }

        /// <summary>
        /// Returns the catalog creator name.
        /// </summary>
        /// <param name="creatorId">the id of the creator</param>
        /// <returns></returns>
        public string GetCreatorName(int creatorId)
        {
            const string sql = "select FullName from Users where Id = @creatorId";
            return Current.DB.Query<string>(sql, new {creatorId}).FirstOrDefault();
        }

        /// <summary>
        /// Fetches catalogs that exist within a series of <see cref="Organization"/>s.
        /// </summary>
        /// <param name="orgs"></param>
        /// <returns></returns>
        public IEnumerable<Catalog> GetCatalogsWithinOrganizations(ICollection<Organization> orgs)
        {
            if (!orgs.Any()) {
                return null;
            }

            const string sql = "select * from Catalogs where OrganizationId in @orgs";
            return Current.DB.Query<Catalog>(sql, new { @orgs = orgs.Select(org => org.Id).ToArray() });
        }

        /// <summary>
        /// Performs the import operations for a catalog.
        /// </summary>
        /// <param name="model">The ViewModel'd container for the import fields and rules.</param>
        /// <param name="currentUserId"></param>
        /// <param name="organizationId"></param>
        /// <returns>The newly created catalog id.</returns>
        public int ImportCatalog(VerifyCatalogImportViewModel model, int currentUserId, int organizationId)
        {
            if (model == null) {
                throw new ArgumentNullException("Catalog import failed due to missing data.");
            }

            var newCatalog = CreateCatalog(model.CatalogInformation, currentUserId);
            var summaries = new List<ICatalogSummaryRecord>();
            var manufacturers = ManufacturerService.GetManufacturers(organizationId).ToList();
            string reason;

            var primaryFields = model.PrimaryCatalogFields;
            var secondaryFields = model.SecondaryCatalogFields;

            for (int i = 0; i < model.Rows.Count() - 1; i++) {
                var row = model.Rows.ElementAt(i);
                Asset asset;
                bool skipProcessing = false;

                // get the manufacturer if they exist, otherwise create it
                // also, to prevent duplicate queries, check existance from 
                // a list. If created, add to said list. Perf.
                var manufacturerName = row[primaryFields.ManufacturerHeaderId];
                var manufacturer = manufacturers.FirstOrDefault(m => m.Name == manufacturerName);
                if (manufacturer == null) {
                    manufacturer = ManufacturerService.CreateManufacturer(manufacturerName, model.CatalogInformation.Organization.Id);
                    manufacturers.Add(manufacturer);
                }
                                   

                // grab all the primary asset fields to check for a match
                var name = row[primaryFields.AssetNameHeaderId];
                var description = row[primaryFields.DescriptionHeaderId];
                var sku = row[primaryFields.SkuHeaderId];

                // does this asset already exist? skip it
                if (AssetService.AssetExists(name, manufacturer.Id, description, sku, newCatalog.Id, out asset)) {
                    summaries.Add(new CatalogRecordImportSkipped(i));
                } else {
                    var newAsset = new Asset
                    {
                        CatalogId = newCatalog.Id,
                        Name = name,
                        ManufacturerId = manufacturer.Id,
                        Description = description,
                        SKU = sku,
                        CreatorId = currentUserId
                    };

                    // is the asset name too long? Flag it. Otherwise, add it.
                    if (AssetService.AssetNameExceedsMaximumLength(name)) {
                        reason = string.Format("Asset name is longer than 250 characters.");
                        summaries.Add(new CatalogRecordImportFailure(i, reason));
                        skipProcessing = true;
                    } else {
                        asset = AssetService.CreateAsset(newAsset, currentUserId);
                    }
                }

                // If the asset would fail to be created, skip the following steps.
                if (skipProcessing) continue;

                // forceful failure if for some reason asset is null
                if (asset == null)
                {
                    throw new Exception("Asset should not be null at this point. Something wrong has occurred.");
                }

                var price = new AssetPrice();

                decimal cost;
                var costRowValue = row[primaryFields.CostHeaderId];
                if (Decimal.TryParse(costRowValue, out cost)) {
                    price.Cost = cost;
                } else {
                    if (costRowValue == "0" || costRowValue.IsNullOrEmpty()) {
                        price.Cost = Decimal.Zero;
                    } else {
                        reason = string.Format("Could not convert cost value of '{0}' to decimal.", costRowValue);
                        summaries.Add(new CatalogRecordImportFailure(i, reason, asset.Id));
                    }
                }

                decimal markup;
                var markupRowValue = row[primaryFields.MarkupHeaderId];
                if (Decimal.TryParse(markupRowValue, out markup)) {
                    price.Markup = markup;
                } else {
                    if (markupRowValue == "0" || markupRowValue.IsNullOrEmpty()) {
                        price.Markup = Decimal.Zero;
                    } else {
                        reason = string.Format("Could not convert markup value of '{0}' to decimal.", markupRowValue);
                        summaries.Add(new CatalogRecordImportFailure(i, reason, asset.Id));
                    }
                }

                price.AssetId = asset.Id;
                price.CatalogId = newCatalog.Id;

                // add the price
                AssetPriceService.InsertPrice(price);

                // And finally let's add in the asset vars
                foreach (var field in secondaryFields.OptionalImportFields)
                {
                    var headerValue = row[field.HeaderId];
                    var varValue = new AssetVarValue(asset.Id, field.AssetVarId, headerValue, organizationId);
                    AssetVarService.InsertVarValue(varValue);
                }

                summaries.Add(new CatalogRecordImportSuccess(i, asset));
            }

            // Insert the import summary
            CatalogSummaryService.InsertSummaries(summaries, newCatalog.Id);

            return newCatalog.Id;
        }
    }
}