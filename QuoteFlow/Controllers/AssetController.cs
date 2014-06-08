using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Dapper;
using QuoteFlow.Infrastructure.Attributes;
using QuoteFlow.Infrastructure.Extensions;
using QuoteFlow.Models;
using QuoteFlow.Models.ViewModels;
using QuoteFlow.Models.ViewModels.Assets;
using QuoteFlow.Services.Interfaces;
using Route = QuoteFlow.Infrastructure.Attributes.RouteAttribute;

namespace QuoteFlow.Controllers
{
    [Authorize]
    public class AssetController : BaseController
    {
        #region IoC

        public IAssetService AssetService { get; protected set; }
        public ICatalogService CatalogService { get; protected set; }
        public IManufacturerService ManufacturerService { get; protected set; }
        public IUserService UserService { get; protected set; }

        public AssetController() { }

        public AssetController(IAssetService assetService, 
            ICatalogService catalogService, 
            IManufacturerService manufacturerService,
            IUserService userService)
        {
            AssetService = assetService;
            CatalogService = catalogService;
            ManufacturerService = manufacturerService;
            UserService = userService;
        }

        #endregion

        [Route("asset/{assetId:INT}/{assetName}")]
        public ActionResult Show(int assetId, string assetName)
        {
            var asset = AssetService.GetAsset(assetId);

            // Ensure that the user has access to the asset
            if (!UserService.CanViewAsset(GetCurrentUser(), asset))
                return PageNotFound();

            var viewModel = new AssetDetailsModel
            {
                Asset = asset,
                Catalog = asset.Catalog
            };

            return asset.Name.UrlFriendly() != assetName ? PageNotFound() : View(viewModel);
        }

        [Route("asset/{assetId:INT}/{assetName}/edit", HttpVerbs.Get)]
        public ActionResult Edit(int assetId, string assetName)
        {
            var asset = AssetService.GetAsset(assetId);
            if (asset == null) 
            {
                return HttpNotFound();
            }

            // Ensure that the user has access to the asset
            if (!UserService.CanViewAsset(GetCurrentUser(), asset))
                return PageNotFound();

            var user = GetCurrentUser();
            
            var manufacturers = ManufacturerService.GetManufacturers(user.Organizations.First().Id);
            var manufacturersDropdown = manufacturers.Select(m => new SelectListItem {Value = m.Id.ToString(), Text = m.Name}).ToList();
            
            var viewModel = new EditAssetRequest
            {
                Id = assetId,
                Name = asset.Name,
                SKU = asset.SKU,
                Description = asset.Description,
                Cost = asset.Cost,
                Markup = asset.Markup,
                ManufacturerId = asset.ManufacturerId,
                Manufacturers = manufacturersDropdown
            };

            return asset.Name.UrlFriendly() != assetName ? PageNotFound() : View(viewModel);
        }

        [Route("asset/{assetId:INT}/{assetName}/edit", HttpVerbs.Post)]
        public ActionResult Edit(int assetId, string assetName, EditAssetRequest form, string returnUrl)
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
                AssetService.UpdateAsset(asset.Id, diff);
            }

            return SafeRedirect(returnUrl ?? Url.Asset(assetId, assetName));
        }

        private static readonly AssetType[] AssetTypeChoices = {
            AssetType.Standard,
            AssetType.Kit
        };

        [Route("asset/new")]
        public ActionResult New()
        {
            var currentUser = GetCurrentUser();
            var catalogs = UserService.GetCatalogs(currentUser);

            var model = new NewAssetModel
            {
                AssetTypeChoices = AssetTypeChoices,
                Catalogs = catalogs.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).OrderBy(s => s.Text)
            };

            return View(model);
        }

        [Route("{catalogId:INT}/{catalogName}/asset/create", HttpVerbs.Post)]
        public ActionResult CreateAsset(int catalogId, string catalogName, NewAssetModel model)
        {
            // Do some server-side validation before we begin
            if (AssetService.AssetExists(model.Name, catalogId))
            {
                // TODO: Return saying that the org name already exists
                return RedirectToAction("New", "Asset");
            }

            var newAsset = AssetService.CreateAsset(model, catalogId, GetCurrentUser().Id);

            //            return RedirectToRoute("catalog", new {
            //                catalogName = newCatalog.Name
            //            });

            // there has to be a better way to do this...
            return Redirect("~/" + catalogId + "/" + catalogName + "/asset/" + newAsset.Id + "/" + newAsset.Name.UrlFriendly());
        }

        [ValidateAntiForgeryToken]
        [Route("asset/{assetId:INT}/{assetName}/addcomment", HttpVerbs.Post)]
        public ActionResult AddComment(NewAssetCommentViewModel model, int assetId, string assetName)
        {
            if (assetId != model.AssetId)
            {
                return PageBadRequest("Asset IDs do not match up");
            }

            AssetService.AddAssetComment(model.Comment, model.AssetId, GetCurrentUser().Id);

            return SafeRedirect(Url.Asset(assetId, assetName));
        }
    }
}
