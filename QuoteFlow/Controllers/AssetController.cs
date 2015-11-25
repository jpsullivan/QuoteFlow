using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Dapper;
using QuoteFlow.Api.Asset;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Auditing;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Models.ViewModels.Assets;
using QuoteFlow.Api.Services;
using QuoteFlow.Api.Util;
using QuoteFlow.Core.Asset.Viewer;
using QuoteFlow.Infrastructure;
using QuoteFlow.Infrastructure.Attributes;
using QuoteFlow.Infrastructure.Extensions;
using QuoteFlow.Infrastructure.Helpers;

namespace QuoteFlow.Controllers
{
    [Authorize]
    public class AssetController : AppController
    {
        #region IoC

        public IAssetService AssetService { get; protected set; }
        public IAssetSearcherManager AssetSearcherManager { get; protected set; }
        public IAssetVarService AssetVarService { get; protected set; }
        public IAuditService AuditService { get; protected set; }
        public ICatalogService CatalogService { get; protected set; }
        public IManufacturerService ManufacturerService { get; protected set; }
        public IUserService UserService { get; protected set; }

        public AssetController()
        {
        }

        public AssetController(IAssetService assetService, IAssetSearcherManager assetSearcherManager, IAssetVarService assetVarService, IAuditService auditService, ICatalogService catalogService, IManufacturerService manufacturerService, IUserService userService)
        {
            AssetService = assetService;
            AssetSearcherManager = assetSearcherManager;
            AssetVarService = assetVarService;
            AuditService = auditService;
            CatalogService = catalogService;
            ManufacturerService = manufacturerService;
            UserService = userService;
        }

        #endregion

        [QuoteFlowRoute("asset/{assetId:INT}/{assetName}")]
        public virtual ActionResult Show(int assetId, string assetName)
        {
            var asset = AssetService.GetAsset(assetId);
            var history = AuditService.GetAssetAuditLogs(assetId);

            // Ensure that the user has access to the asset
            if (!UserService.CanViewAsset(GetCurrentUser(), asset))
            {
                return PageNotFound();
            }

            var viewModel = new AssetDetailsModel(asset, history, false);

            return asset.Name.UrlFriendly() != assetName ? PageNotFound() : View(viewModel);
        }

        [QuoteFlowRoute("asset/{assetId:INT}/{assetName}/edit", HttpVerbs.Get)]
        public virtual ActionResult Edit(int assetId, string assetName)
        {
            var asset = AssetService.GetAsset(assetId);
            if (asset == null) 
            {
                return HttpNotFound();
            }

            // Ensure that the user has access to the asset
            if (!UserService.CanViewAsset(GetCurrentUser(), asset)) 
            {
                return PageNotFound();
            }

            var user = GetCurrentUser();
            
            var manufacturers = ManufacturerService.GetManufacturers(user.Organizations.First().Id);
            var manufacturersDropdown = manufacturers.Select(m => new SelectListItem {Value = m.Id.ToString(), Text = m.Name}).ToList();

            var assetVars = AssetVarService.GetAssetVarsByOrganizationId(CurrentOrganization.Id);
            var assetVarNames = assetVars.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name }).ToList();
            
            var viewModel = new EditAssetRequest
            {
                Id = assetId,
                Name = asset.Name,
                SKU = asset.SKU,
                Description = asset.Description,
                Cost = asset.Cost,
                Markup = asset.Markup,
                AssetVars = asset.AssetVars,
                ManufacturerId = asset.ManufacturerId,
                Manufacturers = manufacturersDropdown,
                AssetVarNames = assetVarNames
            };

            return asset.Name.UrlFriendly() != assetName ? PageNotFound() : View(viewModel);
        }

        [QuoteFlowRoute("asset/{assetId:INT}/{assetName}/edit", HttpVerbs.Post)]
        public virtual ActionResult Edit(int assetId, string assetName, EditAssetRequest form, string returnUrl)
        {
            var asset = AssetService.GetAsset(assetId);
            if (asset == null) 
            {
                return HttpNotFound();
            }

            // ensure that the provided asset matches the expected asset
            if (asset.Id != assetId || asset.Name.UrlFriendly() != assetName)
                return SafeRedirect(returnUrl ?? Url.Asset(assetId, assetName));

            if (!ModelState.IsValid)
            {
                // todo: show some kind of form validation error
            }

            var snapshot = Snapshotter.Start(asset);
            asset.Name = form.Name;
            asset.SKU = form.SKU;
            asset.Description = form.Description;
            asset.Cost = form.Cost;
            asset.Markup = form.Markup;
            asset.LastUpdated = DateTime.UtcNow;

            var diff = snapshot.Diff();
            if (diff.ParameterNames.Any())
            {
                AssetService.UpdateAsset(asset.Id, diff, GetCurrentUser().Id);
            }

            foreach (var assetVarValue in form.AssetVarValuesData)
            {
                AssetVarService.UpdateAssetVarValue(assetVarValue.AssetVarValueId, assetVarValue.AssetVarId, assetVarValue.AssetVarValue);
            }

            return SafeRedirect(returnUrl ?? Url.Asset(assetId, assetName));
        }

        private static readonly AssetType[] AssetTypeChoices = {
            AssetType.Standard,
            AssetType.Kit
        };

        [QuoteFlowRoute("asset/new", Name = RouteNames.AssetNew)]
        public virtual ActionResult New(int? catalogId)
        {
            var currentUser = GetCurrentUser();

            var catalogs = UserService.GetCatalogs(currentUser);
            var manufacturers = ManufacturerService.GetManufacturers(1); // todo organization fix

            var model = new NewAssetModel
            {
                AssetTypeChoices = AssetTypeChoices,
                Catalogs = catalogs.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).OrderBy(s => s.Text),
                Manufacturers = manufacturers
            };

            if (catalogId.HasValue)
            {
                model.CatalogId = (int) catalogId;
            }

            return View(model);
        }

        [QuoteFlowRoute("asset/create", HttpVerbs.Post)]
        public virtual ActionResult CreateAsset(NewAssetModel model)
        {
            // Do some server-side validation before we begin
            if (AssetService.AssetExists(model.Name, model.CatalogId))
            {
                // TODO: Return saying that the org name already exists
                return RedirectToAction("New", "Asset");
            }

            var currentUser = GetCurrentUser();

            // check if the mfg exists, if not then add it
            int manufacturerId;
            var manufacturer = ManufacturerService.GetManufacturer(model.Manufacturer, true);
            if (manufacturer == null)
            {
                var createdMfg = ManufacturerService.CreateManufacturer(model.Manufacturer, 1); // todo organization fix
                manufacturerId = createdMfg.Id;

            }
            else
            {
                manufacturerId = manufacturer.Id;
            }
            
            var asset = new Asset
            {
                Name = model.Name,
                SKU = model.SKU,
                Description = model.Description,
                CatalogId = model.CatalogId,
                ManufacturerId = manufacturerId,
                Type = "Asset",
                Cost = model.Cost,
                Markup = (model.Markup / 100),
                CreatorId = currentUser.Id
            };

            var newAsset = AssetService.CreateAsset(asset, GetCurrentUser().Id, false);

            return SafeRedirect(Url.Asset(newAsset.Id, newAsset.Name));
        }

        [ValidateAntiForgeryToken]
        [QuoteFlowRoute("asset/{assetId:INT}/{assetName}/addcomment", HttpVerbs.Post)]
        public virtual ActionResult AddComment(NewAssetCommentViewModel model, int assetId, string assetName)
        {
            if (assetId != model.AssetId)
            {
                return PageBadRequest("Asset IDs do not match up");
            }

            AssetService.AddAssetComment(model.Comment, model.AssetId, GetCurrentUser().Id);

            return SafeRedirect(Url.Asset(assetId, assetName));
        }

        [QuoteFlowRoute("asset/builder/getAsset", HttpVerbs.Get)]
        public ActionResult GetAsset(int assetId, string decorator, bool prefetch, bool shouldUpdateCurrentCatalog, bool loadFields)
        {
            var errors = new SimpleErrorCollection();
            var asset = AssetService.GetAsset(assetId);
            var history = AuditService.GetAssetAuditLogs(asset.Id);

            var activityFeed = this.RenderRazorViewToString("Partials/Auditing/ActivityFeed", history);
            asset.AssetHistoryHtml = activityFeed;

            var result = new AssetResult(asset, errors);

            if (!result.IsValid() && result.Asset == null)
            {
                var errorCollection = result.ErrorCollection;
                return new JsonCamelCaseResult(new AssetFields("todo_generate_xsrf_token", errorCollection), JsonRequestBehavior.AllowGet);
            }

            if (!prefetch)
            {
                // todo add asset to some sort of history tracker

                // don't set the selected catalog if we are looking at the detail view in asset nav
                if (shouldUpdateCurrentCatalog)
                {
                    // todo: set the selected catalog
                }
            }

            PopulateAssetFields(asset, false, result.ErrorCollection);

            var fields = new AssetFields("todo_changeme", result.ErrorCollection, new List<string>(), asset, null);

            return new JsonCamelCaseResult(fields, JsonRequestBehavior.AllowGet);
        }

        [QuoteFlowRoute("asset/builder/getAssetMergeCurrent", HttpVerbs.Post)]
        public ActionResult GetAssetMergeCurrent(MergeIntoCurrent mic)
        {
            return GetAsset(mic.AssetId, mic.Decorator, mic.Prefetch, mic.ShouldUpdateCurrentCatalog, false);
        }

        private void PopulateAssetFields(IAsset asset, bool retainValues, IErrorCollection errorCollection)
        {
            //var isEditable = AssetService.IsAssetEditable(asset, RequestContext.Principal.Identity.Name);

            var editFields = new List<string>();
        }

        public class MergeIntoCurrent
        {
            public int AssetId { get; set; }
            public string Asset { get; set; }
            public string Decorator { get; set; }
            public bool Prefetch { get; set; }
            public bool ShouldUpdateCurrentCatalog { get; set; }
            public long LastReadTime { get; set; }
        }
    }
}
