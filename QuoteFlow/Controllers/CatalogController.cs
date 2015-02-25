using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CsvHelper;
using Jil;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Infrastructure.Helpers;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Models.CatalogImport;
using QuoteFlow.Api.Models.ViewModels;
using QuoteFlow.Api.Services;
using QuoteFlow.Api.Upload;
using QuoteFlow.Infrastructure.AsyncFileUpload;
using QuoteFlow.Infrastructure.Attributes;
using QuoteFlow.Infrastructure.Extensions;
using QuoteFlow.Infrastructure.Helpers;
using QuoteFlow.Services;
using Route = QuoteFlow.Infrastructure.Attributes.RouteAttribute;

namespace QuoteFlow.Controllers
{
    [Authorize]
    public partial class CatalogController : AppController
    {
        #region DI

        public IAssetService AssetService { get; protected set; }
        public ICatalogService CatalogService { get; protected set; }
        public ICatalogImportSummaryRecordsService CatalogImportSummaryService { get; set; }
        public ICatalogImportService CatalogImportService { get; protected set; }
        public IOrganizationService OrganizationService { get; protected set; }
        public IUserService UserService { get; protected set; }
        public IUploadFileService UploadFileService { get; protected set; }
        public ICacheService CacheService { get; protected set; }

        public CatalogController() { }

        public CatalogController(IAssetService assetService, ICatalogService catalogService, 
            ICatalogImportService catalogImportService, ICatalogImportSummaryRecordsService catalogImportSummaryService,
            IOrganizationService organizationService, IUserService userService,
            IUploadFileService uploadFileService, ICacheService cacheService)
        {
            AssetService = assetService;
            CatalogService = catalogService;
            CatalogImportService = catalogImportService;
            CatalogImportSummaryService = catalogImportSummaryService;
            OrganizationService = organizationService;
            UserService = userService;
            UploadFileService = uploadFileService;
            CacheService = cacheService;
        }

        #endregion

        [Route("catalogs")]
        public virtual ActionResult Index()
        {
            return View();
        }

        [Route("catalog/new", HttpVerbs.Get)]
        public virtual ActionResult New()
        {
            // if user has more than one organization, get them
            // display them as select list
            //ViewData["UserOrganizations"] = Organization.GetOrganizations(Current.User.Id);

            return View();
        }

        [Route("catalog/create", HttpVerbs.Post)]
        public virtual ActionResult CreateCatalog(NewCatalogModel model)
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
        public virtual ActionResult Show(int catalogId, string catalogName)
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
        public virtual ActionResult ShowAssets(int catalogId, string catalogName, int? page)
        {
            var catalog = CatalogService.GetCatalog(catalogId);
            if (catalog == null) {
                return PageNotFound();
            }

            const int perPage = 50;
            var currentPage = Math.Max(page ?? 1, 1);

            var creator = UserService.GetUser(catalog.CreatorId);
            var assets = AssetService.GetAssets(catalog.Id).ToList();
            var pagedAssets = assets.ToPagedList(currentPage, perPage);
            var paginationUrl = Url.CatalogAssets(catalog.Id, catalog.Name.UrlFriendly(), -1);

            var model = new CatalogShowAssetsModel
            {
                Assets = pagedAssets,
                Catalog = catalog,
                CatalogCreator = creator,
                CurrentPage = currentPage,
                PaginationUrl = paginationUrl
            };

            return catalog.Name.UrlFriendly() != catalogName ? PageNotFound() : View(model);
        }

//        [Route("catalog/{catalogId:INT}/{catalogName}/assets/iv")]
//        public virtual ActionResult ShowAssetsInteractive(int catalogId, string catalogName, int? page)
//        {
//            var catalog = CatalogService.GetCatalog(catalogId);
//            if (catalog == null)
//            {
//                return PageNotFound();
//            }
//
//            const int perPage = 50;
//            var currentPage = Math.Max(page ?? 1, 1);
//
//            var creator = UserService.GetUser(catalog.CreatorId);
//            var assets = AssetService.GetAssets(catalog.Id).ToList();
//            var pagedAssets = assets.ToPagedList(currentPage, perPage);
//            var paginationUrl = Url.CatalogAssetsInteractive(catalog.Id, catalog.Name.UrlFriendly(), -1);
//
//            // build up the navigation filter items
//            var manufacturers = assets.Select(a => a.Manufacturer).ToList();
//            manufacturers = manufacturers.Distinct(m => m.Id).ToList();
//            var creators = UserService.GetUsers(CurrentOrganization.Id);
//
//            // populate the comments section for the first asset
//            // todo: this is a total hack and sucks entirely. Not sure what I even want to do here.
//            pagedAssets[0] = AssetService.GetAsset(pagedAssets.First().Id);
//
//            var model = new CatalogShowAssetsModel
//            {
//                AssetCreators = creators,
//                Assets = pagedAssets,
//                Catalog = catalog,
//                CatalogCreator = creator,                
//                CurrentPage = currentPage,
//                Manufacturers = manufacturers,
//                PaginationUrl = paginationUrl
//            };
//
//            return catalog.Name.UrlFriendly() != catalogName ? PageNotFound() : View(model);
//        }

        [Route("catalog/{catalogId:INT}/{catalogName}/import-results/{filter?}")]
        public virtual async Task<ActionResult> ShowImportSummary(int catalogId, string catalogName, string filter, int? page)
        {
            var catalog = CatalogService.GetCatalog(catalogId);
            if (catalog == null) {
                return PageNotFound();
            }

            const int perPage = 50;
            var currentPage = Math.Max(page ?? 1, 1);

            var creator = UserService.GetUser(catalog.CreatorId);
            var summary = CatalogImportSummaryService.GetImportSummaryRecords(catalog.Id).ToList();

            int totalSuccess = summary.Count(s => s.Result == CatalogSummaryResult.Success);
            int totalSkipped = summary.Count(s => s.Result == CatalogSummaryResult.Skip);
            int totalFailed = summary.Count(s => s.Result == CatalogSummaryResult.Failure);

            // handle any filters that may exist, as well as build the correct pagination url
            var paginationUrl = String.Empty;
            if (filter.HasValue()) {
                if (filter == "successful") {
                    summary = summary.Where(row => row.Result == CatalogSummaryResult.Success).ToList();
                    paginationUrl = Url.CatalogImportResultsSuccess(catalog.Id, catalog.Name.UrlFriendly(), -1);
                }
                if (filter == "skipped") {
                    summary = summary.Where(row => row.Result == CatalogSummaryResult.Skip).ToList();
                    paginationUrl = Url.CatalogImportResultsSkipped(catalog.Id, catalog.Name.UrlFriendly(), -1);
                }
                if (filter == "failed") {
                    summary = summary.Where(row => row.Result == CatalogSummaryResult.Failure).ToList();
                    paginationUrl = Url.CatalogImportResultsFailed(catalog.Id, catalog.Name.UrlFriendly(), -1);
                }
            } else {
                paginationUrl = Url.CatalogImportResults(catalog.Id, catalog.Name.UrlFriendly(), -1);
            }

            var rawSummary = CatalogImportSummaryService.ConvertToRawSummaryRecords(summary, catalog.Id).ToPagedList(currentPage, perPage);

            var model = new CatalogShowImportSummaryModel
            {
                Summary = rawSummary,
                Catalog = catalog,
                CatalogCreator = creator,
                CurrentPage = currentPage,
                Filter = filter,
                PaginationUrl = paginationUrl,
                TotalSuccess = string.Format("{0:n0}", totalSuccess),
                TotalSkipped = string.Format("{0:n0}", totalSkipped),
                TotalFailed = string.Format("{0:n0}", totalFailed)
            };

            return catalog.Name.UrlFriendly() != catalogName ? PageNotFound() : View(model);
        }

        [Route("catalog/{catalogId:INT}/{catalogName}/admin")]
        public virtual ActionResult Admin(int catalogId, string catalogName)
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
                    return RedirectToAction("SetImportCatalogDetails");
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
                await UploadFileService.SaveUploadFileAsync(currentUser.Id, uploadStream, Path.GetExtension(uploadFile.FileName), UploadType.Catalog);
            }

            return RedirectToAction("SetImportCatalogDetails", "Catalog");
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public virtual ActionResult ImportCatalogProgress()
        {
            string username = User.Identity.Name;

            AsyncFileUploadProgress progress = CacheService.GetProgress(username);
            if (progress == null)
            {
                return HttpNotFound();
            }
            return Json(progress, JsonRequestBehavior.AllowGet);
        }

        [Route("catalog/importCatalogDetails", HttpVerbs.Get)]
        public virtual async Task<ActionResult> SetImportCatalogDetails()
        {
            return View();
        }

        [Route("catalog/importCatalogDetails", HttpVerbs.Post)]
        public virtual async Task<ActionResult> SetImportCatalogDetails(NewCatalogModel catalogDetails)
        {
            TempData["CatalogDetails"] = catalogDetails;

            return RedirectToAction("VerifyImport");
        }

        [Route("catalog/verify", HttpVerbs.Get)]
        public virtual async Task<ActionResult> VerifyImport()
        {
            var catalogDetails = (NewCatalogModel) TempData["CatalogDetails"];
            if (catalogDetails == null) 
            {
                return RedirectToAction("Import");
            }

            // todo: ensure that the catalog name is not already taken
            if (CatalogService.CatalogNameExists(catalogDetails.Name, CurrentOrganization.Id))
            {
                return PageBadRequest("Catalog name already taken. Show this error in the form.");
            }

            catalogDetails.Organization = CurrentOrganization;

            // fetch the catalog data
            var catalogData = GetCatalogData().Result;
            if (catalogData == null) {
                return RedirectToAction("Import", "Catalog");
            }

            var headers = catalogData.Headers;
            var rows = catalogData.Rows;

            var model = new VerifyCatalogImportViewModel
            {
                Headers = headers,
                Rows = rows,
                TotalRows = rows.Count(),
                CatalogInformation = catalogDetails
            };

            return View(model);
        }

        [Route("catalog/verify", HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> VerifyImport(VerifyCatalogImportViewModel formData)
        {
            var catalogInformation = JSON.Deserialize<NewCatalogModel>(Request.Form["CatalogInformation"]);

            // todo: validate the previous form to ensure that no two dropdowns have the same value selected

            // todo: determine which headers have been selected, and remove them from the dropdown
            // todo: fetch a list of all asset vars to be placed into a dropdown of their own

            // rather than take the entire viewmodel with all the rows to the client, just 
            // take the necessary ones
            var slimVerifyModel = new VerifyCatalogImportViewModel
            {
                CatalogInformation = catalogInformation,
                PrimaryCatalogFields = formData.PrimaryCatalogFields
            };
            TempData["VerifyModel"] = slimVerifyModel;

            return RedirectToAction("VerifyImportSecondary", slimVerifyModel);
        }

        [Route("catalog/verifyOther", HttpVerbs.Get)]
        public virtual async Task<ActionResult> VerifyImportSecondary()
        {
            var slimVerifyModel = (VerifyCatalogImportViewModel)TempData["VerifyModel"];
            // Since we can't get proper modelbinding on a multi-step form, we are 
            // going to have to just get greasy and re-fetch the csv data... This class is fucked.

            var catalogData = GetCatalogData().Result;
            if (catalogData == null) 
            {
                return RedirectToAction("Import");
            }

            var headers = catalogData.Headers;
            var rows = catalogData.Rows;

            var model = new VerifyCatalogImportViewModel
            {
                Headers = headers,
                Rows = rows,
                TotalRows = rows.Count(),
                CatalogInformation = slimVerifyModel.CatalogInformation,
                PrimaryCatalogFields = slimVerifyModel.PrimaryCatalogFields
            };

            return View(model);
        }

        [Route("catalog/verifyOther", HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> VerifyImportSecondary(FormCollection form)
        {
            var currentUser = GetCurrentUser();

            var catalogInformation = JSON.Deserialize<NewCatalogModel>(form["CatalogInformation"]);
            var primaryFields = JSON.Deserialize<PrimaryCatalogFieldsViewModel>(form["PrimaryCatalogFields"]);
            var fields = new List<OptionalImportField>();

            foreach (var key in form.AllKeys) 
            {
                if (!key.StartsWith("AssetVarId_", StringComparison.CurrentCultureIgnoreCase)) continue;

                var assetVarId = Int32.Parse(form[key]);
                var headerId = Int32.Parse(form["HeaderId_" + assetVarId]); // directly fetch the header value for this asset var
                fields.Add(new OptionalImportField(assetVarId, headerId));
            }

            var secondaryFields = new SecondaryCatalogFieldsViewModel(fields);

            // re-fetch the catalog data because this class is fucking horrendous
            var catalogData = GetCatalogData().Result;
            if (catalogData == null)
            {
                return RedirectToAction("Import");
            }

            var headers = catalogData.Headers;
            var rows = catalogData.Rows;

            var model = new VerifyCatalogImportViewModel
            {
                Headers = headers,
                Rows = rows,
                TotalRows = rows.Count(),
                CatalogInformation = catalogInformation,
                PrimaryCatalogFields = primaryFields,
                SecondaryCatalogFields = secondaryFields
            };

            // Do the import!
            var id = CatalogImportService.ImportCatalog(model, currentUser.Id, CurrentOrganization.Id);

            await UploadFileService.DeleteUploadFileAsync(currentUser.Id);

            var url = Url.CatalogImportResults(id, CatalogService.GetCatalog(id).Name.UrlFriendly());
            return Redirect(url);
        }

        [Route("catalog/cancelImport")]
        public virtual async Task<ActionResult> CancelImport()
        {
            var currentUser = GetCurrentUser();
            await UploadFileService.DeleteUploadFileAsync(currentUser.Id);

            return RedirectToAction("Import");
        }

        [NonAction]
        public async Task<CatalogCsv> GetCatalogData()
        {
            var currentUser = GetCurrentUser();

            var headers = new List<SelectListItem>();
            string[] rawHeaders = { };
            var rows = new List<string[]>();

            using (Stream uploadFile = await UploadFileService.GetUploadFileAsync(currentUser.Id, UploadType.Catalog.GetUploadFolder()))
            {
                if (uploadFile == null) {
                    return null;
                }

                try
                {
                    var sr = new StreamReader(uploadFile);
                    var csvReader = new CsvReader(sr);
                    csvReader.Configuration.HasHeaderRecord = true;

                    while (csvReader.Read())
                    {
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
                headers.Add(new SelectListItem { Value = i.ToString(), Text = rawHeaders[i] });
            }

            return new CatalogCsv {Headers = headers, Rows = rows};
        }
    }
}