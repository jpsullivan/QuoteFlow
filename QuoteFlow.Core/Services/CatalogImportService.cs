using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Models.CatalogImport;
using QuoteFlow.Api.Models.ViewModels;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Core.Services
{
    public class CatalogImportService : ICatalogImportService
    {
        #region IoC

        public IAssetService AssetService { get; protected set; }
        public IAssetVarService AssetVarService { get; protected set; }
        public ICatalogImportSummaryRecordsService CatalogSummaryService { get; protected set; }
        public ICatalogService CatalogService { get; protected set; }
        public IManufacturerService ManufacturerService { get; protected set; }

        public CatalogImportService(IAssetService assetService, IAssetVarService assetVarService, 
            ICatalogImportSummaryRecordsService summaryRecordsService, ICatalogService catalogService, IManufacturerService manufacturerService)
        {
            AssetService = assetService;
            AssetVarService = assetVarService;
            CatalogSummaryService = summaryRecordsService;
            CatalogService = catalogService;
            ManufacturerService = manufacturerService;
        }

        #endregion

        /// <summary>
        /// Performs the import operations for a catalog.
        /// </summary>
        /// <param name="model">The ViewModel'd container for the import fields and rules.</param>
        /// <param name="currentUserId"></param>
        /// <param name="organizationId"></param>
        /// <returns>The newly created catalog id.</returns>
        public int ImportCatalog(VerifyCatalogImportViewModel model, int currentUserId, int organizationId)
        {
            if (model == null)
            {
                throw new ArgumentNullException("Catalog import failed due to missing data.");
            }

            var newCatalog = CatalogService.CreateCatalog(model.CatalogInformation, currentUserId);
            var summaries = new List<ICatalogSummaryRecord>();
            var manufacturers = ManufacturerService.GetManufacturers(organizationId).ToList();
            string reason;

            var primaryFields = model.PrimaryCatalogFields;
            var secondaryFields = model.SecondaryCatalogFields;

            for (int i = 0; i < model.Rows.Count() - 1; i++)
            {
                var row = model.Rows.ElementAt(i);
                Api.Models.Asset asset;

                // get the manufacturer if they exist, otherwise create it
                // also, to prevent duplicate queries, check existance from 
                // a list. If created, add to said list. Perf.
                var manufacturerName = row[primaryFields.ManufacturerHeaderId];
                var manufacturer = manufacturers.FirstOrDefault(m => m.Name == manufacturerName);
                if (manufacturer == null)
                {
                    manufacturer = ManufacturerService.CreateManufacturer(manufacturerName, model.CatalogInformation.Organization.Id);
                    manufacturers.Add(manufacturer);
                }

                // grab all the primary asset fields to check for a match
                var name = row[primaryFields.AssetNameHeaderId];
                var description = row[primaryFields.DescriptionHeaderId];
                var sku = row[primaryFields.SkuHeaderId];

                // was this asset already added during the import session?
                if (AssetService.AssetExists(sku, manufacturer.Id, newCatalog.Id, out asset))
                {
                    var msg = string.Format("Asset '{0}', seems to already exist.", asset.SKU);
                    summaries.Add(new CatalogRecordImportSkipped(i, msg));
                    continue;
                }

                // is the asset name too long? Flag it. Otherwise, add it.
                if (AssetService.AssetNameExceedsMaximumLength(name))
                {
                    reason = string.Format("Asset name is longer than 250 characters.");
                    summaries.Add(new CatalogRecordImportFailure(i, reason));
                    continue;
                }

                var newAsset = new Api.Models.Asset()
                {
                    Name = name,
                    SKU = sku,
                    Description = description,
                    CatalogId = newCatalog.Id,
                    ManufacturerId = manufacturer.Id,
                    CreatorId = currentUserId
                };

                decimal cost;
                var costRowValue = row[primaryFields.CostHeaderId];
                if (Decimal.TryParse(costRowValue, out cost))
                {
                    newAsset.Cost = cost;
                }
                else
                {
                    if (costRowValue != "0" && !string.IsNullOrEmpty(costRowValue))
                    {
                        reason = string.Format("Could not convert cost value of '{0}' to decimal.", costRowValue);
                        summaries.Add(new CatalogRecordImportFailure(i, reason));
                        continue;
                    }
                    newAsset.Cost = Decimal.Zero;
                }

                decimal markup;
                var markupRowValue = row[primaryFields.MarkupHeaderId];
                if (Decimal.TryParse(markupRowValue, out markup))
                {
                    newAsset.Markup = markup;
                }
                else
                {
                    if (markupRowValue != "0" && !string.IsNullOrEmpty(markupRowValue))
                    {
                        reason = string.Format("Could not convert markup value of '{0}' to decimal.", markupRowValue);
                        summaries.Add(new CatalogRecordImportFailure(i, reason));
                        continue;
                    }
                    newAsset.Markup = Decimal.Zero;
                }

                asset = AssetService.CreateAsset(newAsset, currentUserId);

                // forceful failure if for some reason asset is null
                if (asset == null)
                {
                    throw new Exception("Asset should not be null at this point. Something wrong has occurred.");
                }

                // Finally insert the asset vars
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