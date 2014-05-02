using System.Linq;
using System.Web.Mvc;
using QuoteFlow.Infrastructure.Attributes;
using QuoteFlow.Infrastructure.Extensions;
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

        [Route("asset/{assetId:INT}/{assetName}")]
        public ActionResult Show(int assetId, string assetName)
        {
            var asset = AssetService.GetAsset(assetId);

            // Ensure that the user has access to the supplied catalog
            if (!UserService.IsCatalogMember(GetCurrentUser().Id, asset.CatalogId)) return PageNotFound();

            var viewModel = new AssetDetailsModel
            {
                Asset = asset,
                Catalog = CatalogService.GetCatalog(asset.CatalogId)
            };

            return asset.Name.UrlFriendly() != assetName ? PageNotFound() : View(viewModel);
        }

        private static readonly AssetType[] AssetTypeChoices = new[]
        {
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
    }
}
