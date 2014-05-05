using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CsvHelper;
using QuoteFlow.Infrastructure;
using QuoteFlow.Infrastructure.AsyncFileUpload;
using QuoteFlow.Infrastructure.Attributes;
using QuoteFlow.Infrastructure.Extensions;
using QuoteFlow.Models;
using QuoteFlow.Models.ViewModels;
using QuoteFlow.Services.Interfaces;
using Route = QuoteFlow.Infrastructure.Attributes.RouteAttribute;

namespace QuoteFlow.Controllers
{
    [Authorize]
    public class CatalogController : BaseController
    {
        #region IoC

        public IAssetService AssetService { get; protected set; }
        public ICatalogService CatalogService { get; protected set; }
        public IOrganizationService OrganizationService { get; protected set; }
        public IUserService UserService { get; protected set; }
        public IUploadFileService UploadFileService { get; protected set; }
        public ICacheService CacheService { get; protected set; }

        public CatalogController() { }

        public CatalogController(IAssetService assetService, ICatalogService catalogService, 
            IOrganizationService organizationService, IUserService userService,
            IUploadFileService uploadFileService, ICacheService cacheService)
        {
            AssetService = assetService;
            CatalogService = catalogService;
            OrganizationService = organizationService;
            UserService = userService;
            UploadFileService = uploadFileService;
            CacheService = cacheService;
        }

        #endregion

        [Route("catalogs")]
        public ActionResult Index()
        {
            return View();
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

        [Route("catalog/{catalogId:INT}/{catalogName}")]
        public ActionResult Show(int catalogId, string catalogName)
        {
            var catalog = CatalogService.GetCatalog(catalogId);
            if (catalog == null)
            {
                return PageNotFound();
            }

            var creator = UserService.GetUser(catalog.CreatorId);
            var assets = AssetService.GetAssets(catalog);

            var model = new CatalogShowModel
            {
                Assets = assets,
                Catalog = catalog,
                CatalogCreator = creator
            };

            return catalog.Name.UrlFriendly() != catalogName ? PageNotFound() : View(model);
        }

        [Route("catalog/{catalogId:INT}/{catalogName}/assets")]
        public ActionResult ShowAssets(int catalogId, string catalogName)
        {
            var catalog = CatalogService.GetCatalog(catalogId);
            if (catalog == null)
            {
                return PageNotFound();
            }

            var assets = AssetService.GetAssets(catalog);

            var model = new CatalogShowAssetsModel
            {
                Assets = assets,
                Catalog = catalog
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

        [Route("catalog/import", HttpVerbs.Get)]
        public async virtual Task<ActionResult> Import()
        {
            var currentUser = GetCurrentUser();

            using (var existingUploadFile = await UploadFileService.GetUploadFileAsync(currentUser.Id))
            {
                if (existingUploadFile != null) {
                    return RedirectToAction("VerifyImport");
                }
            }

            return View();
        }

        [Route("catalog/import", HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Import(HttpPostedFileBase uploadFile)
        {
            var currentUser = GetCurrentUser();

            using (var existingUploadFile = await UploadFileService.GetUploadFileAsync(currentUser.Id))
            {
                if (existingUploadFile != null)
                {
                    return new HttpStatusCodeResult(409, "Cannot upload file because an upload is already in progress.");
                }
            }

            if (uploadFile == null)
            {
                ModelState.AddModelError(String.Empty, Strings.CatalogImportFileRequired);
                return View();
            }

            using (var uploadStream = uploadFile.InputStream)
            {
                await UploadFileService.SaveUploadFileAsync(currentUser.Id, uploadStream, Path.GetExtension(uploadFile.FileName));
            }

            return RedirectToAction("VerifyImport", "Catalog");
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult ImportCatalogProgress()
        {
            string username = User.Identity.Name;

            AsyncFileUploadProgress progress = CacheService.GetProgress(username);
            if (progress == null)
            {
                return HttpNotFound();
            }
            return Json(progress, JsonRequestBehavior.AllowGet);
        }

        [Route("catalog/verify", HttpVerbs.Get)]
        public virtual async Task<ActionResult> VerifyImport()
        {
            var currentUser = GetCurrentUser();

            var headers = new List<SelectListItem>();
            string[] rawHeaders = {};
            var rows = new List<string[]>();

            using (Stream uploadFile = await UploadFileService.GetUploadFileAsync(currentUser.Id))
            {
                if (uploadFile == null)
                {
                    return RedirectToAction("Import", "Catalog");
                }

                try
                {
                    var sr = new StreamReader(uploadFile);
                    var csvReader = new CsvReader(sr);
                    csvReader.Configuration.HasHeaderRecord = true;

                    while (csvReader.Read()) {
                        rawHeaders = csvReader.FieldHeaders;

                        var row = new string[rawHeaders.Count()];
                        for (int i = 0; i < rawHeaders.Count() - 1; i++)
                        {
                            row[i] = csvReader.GetField(i);
                        }
                        rows.Add(row);
                    }
                }
                catch (InvalidDataException e)
                {
                    throw new InvalidDataException();
                }
            }

            // pretty up the headers into a convenient dropdown list
            for (int i = 0; i < rawHeaders.Count(); i++)
            {
                headers.Add(new SelectListItem { Value = i.ToString(), Text = rawHeaders[i]});
            }

            var model = new VerifyCatalogImportViewModel
            {
                Headers = headers,
                Rows = rows,
                TotalRows = rows.Count()
            };

            return View(model);
        }

        [Route("catalog/verify", HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> VerifyImport(VerifyCatalogImportViewModel formData)
        {
            // todo: validate the previous form to ensure that no two dropdowns have the same value selected

            // todo: determine which headers have been selected, and remove them from the dropdown
            // todo: fetch a list of all asset vars to be placed into a dropdown of their own

            
            return new EmptyResult();
        }

        [Route("catalog/cancelImport", HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> CancelImport()
        {
            var currentUser = GetCurrentUser();
            await UploadFileService.DeleteUploadFileAsync(currentUser.Id);

            return RedirectToAction("Import");
        }
    }
}