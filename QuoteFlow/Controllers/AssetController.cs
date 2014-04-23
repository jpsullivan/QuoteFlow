using System.Web.Mvc;
using QuoteFlow.Infrastructure.Attributes;
using QuoteFlow.Infrastructure.Extensions;
using QuoteFlow.Infrastructure.Helpers;
using QuoteFlow.Models;
using QuoteFlow.Models.ViewModels;
using QuoteFlow.Services.Interfaces;
using Route = QuoteFlow.Infrastructure.Attributes.RouteAttribute;

namespace QuoteFlow.Controllers
{
    public class AssetController : BaseController
    {
        #region IoC

        public IAssetService AssetService { get; protected set; }
        public ICatalogService CatalogService { get; protected set; }
        public IUserService UserService { get; protected set; }

        public AssetController() { }

        public AssetController(IAssetService assetService, ICatalogService catalogService, IUserService userService)
        {
            AssetService = assetService;
            CatalogService = catalogService;
            UserService = userService;
        }

        #endregion

        [Route("{catalogId:INT}/{catalogName}/asset/{assetId:INT}/{assetName}")]
        public ActionResult Show(int catalogId, int assetId, string assetName)
        {
            var catalog = CatalogService.GetCatalog(catalogId);

            // Ensure that the user has access to the supplied catalog
            if (!UserService.IsCatalogMember(GetCurrentUser(), catalog)) return PageNotFound();

            var asset = AssetService.GetAsset(assetId);

            var viewModel = new AssetDetailsModel
            {
                Asset = asset,
                Catalog = catalog
            };

            return asset.Name.UrlFriendly() != assetName ? PageNotFound() : View(viewModel);
        }

        [Route("{catalogId:INT}/{catalogName}/asset/new")]
        [LayoutInjector("_LayoutWorkflow")]
        public ActionResult New(int catalogId, string catalogName)
        {
            var catalog = CatalogService.GetCatalog(catalogId);

            if (catalog == null)
            {
                return PageNotFound();
            }

            ViewData["catalog"] = catalog;

            return catalog.Name.UrlFriendly() != catalogName ? PageNotFound() : View();
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
    }
}
