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
    public class CatalogController : BaseController
    {
        #region IoC

        public IAssetService AssetService { get; protected set; }
        public ICatalogService CatalogService { get; protected set; }
        public IOrganizationService OrganizationService { get; protected set; }
        public IUserService UserService { get; protected set; }

        public CatalogController() { }

        public CatalogController(IAssetService assetService, ICatalogService catalogService, 
            IOrganizationService organizationService, IUserService userService)
        {
            AssetService = assetService;
            CatalogService = catalogService;
            OrganizationService = organizationService;
            UserService = userService;
        }

        #endregion

        [Route("catalogs")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("catalog/{catalogId:INT}/{catalogName}")]
        public ActionResult Show(int catalogId, string catalogName)
        {
            var catalog = CatalogService.GetCatalog(catalogId);
            if (catalog == null)
            {
                return PageNotFound();
            }

            var assets = AssetService.GetAssets(catalog);

            var model = new CatalogShowModel
            {
                Assets = assets,
                Catalog = catalog,
                SubHeader = (SubHeader)ViewData["SubHeader"]
            };

            return catalog.Name.UrlFriendly() != catalogName ? PageNotFound() : View(model);
        }

        [Route("catalog/new", HttpVerbs.Get)]
        public ActionResult New()
        {
            // if user has more than one organization, get them
            // display them as select list
            //ViewData["UserOrganizations"] = Organization.GetOrganizations(Current.User.Id);

            return View();
        }

        [Route("catalog/create", HttpVerbs.Post)]
        public ActionResult CreateCatalog(NewCatalogModel model)
        {
            var currentUser = GetCurrentUser();

            // lazily set the organization to the first one that the user is assigned to.
            // eventually use whatever the users' current organization is once multi-tenancy
            // is implemented.
            model.Organization = currentUser.Organizations.First();

            // Do some server-side validation before we begin
            if (CatalogService.CatalogNameExists(model.Name, model.Organization.Id)) 
            {
                var errorMsg = string.Format("Catalog name already exists within the {0} organization.",
                    model.Organization.OrganizationName);
                ModelState.AddModelError("Name", errorMsg);
                return View("New", model);
            }

            var newCatalog = CatalogService.CreateCatalog(model, currentUser.Id);

            // there has to be a better way to do this...
            return Redirect("~/catalog/" + newCatalog.Id + "/" + newCatalog.Name.UrlFriendly());
        }

        //        [Route("catalog/{catalogId:INT}/{catalogName}/quotes")]
        //        public ActionResult ShowCatalogQuotes(int catalogId, string catalogName) { }

        [Route("catalog/{catalogId:INT}/{catalogName}/assets")]
        public ActionResult ShowAssets(int catalogId, string catalogName)
        {
            var catalog = CatalogService.GetCatalog(catalogId);

            if (catalog == null)
            {
                return PageNotFound();
            }

            var assets = AssetService.GetAssets(catalog);

            var model = new CatalogShowAssets
            {
                Assets = assets,
                Catalog = catalog,
                SubHeader = (SubHeader)ViewData["SubHeader"]
            };

            return catalog.Name.UrlFriendly() != catalogName ? PageNotFound() : View(model);
        }

        [Route("catalog/{catalogId:INT}/{catalogName}/admin")]
        public ActionResult Admin(int catalogId, string catalogName)
        {
            var catalog = CatalogService.GetCatalog(catalogId);

            if (catalog == null)
            {
                return PageNotFound();
            }

            var currentUser = GetCurrentUser();

            // Ensure this user has access to view the catalog admin page
            if (UserService.BelongsToOrganization(currentUser, catalog.Id))
            {
                // Check if the user is a catalog admin
                if (UserService.IsCatalogAdmin(currentUser, catalog.Id)) { }
            }

            return catalog.Name.UrlFriendly() != catalogName ? PageNotFound() : View(catalog);
        }
    }
}