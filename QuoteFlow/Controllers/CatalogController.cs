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

            CsvReader csvReader;
            var headers = new List<SelectListItem>();
            string[] rawHeaders = {};
            string[] row;
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
                    csvReader = new CsvReader(sr);
                    csvReader.Configuration.HasHeaderRecord = true;

                    while (csvReader.Read()) {
                        rawHeaders = csvReader.FieldHeaders;

                        row = new string[rawHeaders.Count()];
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
                headers.Add(new SelectListItem() { Text = rawHeaders[i]});
            }

            var model = new VerifyCatalogImportViewModel
            {
                Headers = headers, 
                Rows = rows.Take(100)
            };

//            var model = new VerifyPackageRequest
//            {
//                Id = packageMetadata.Id,
//                Version = packageMetadata.Version.ToNormalizedStringSafe(),
//                LicenseUrl = packageMetadata.LicenseUrl.ToEncodedUrlStringOrNull(),
//                Listed = true,
//                Edit = new EditPackageVersionRequest
//                {
//                    Authors = packageMetadata.Authors.Flatten(),
//                    Copyright = packageMetadata.Copyright,
//                    Description = packageMetadata.Description,
//                    IconUrl = packageMetadata.IconUrl.ToEncodedUrlStringOrNull(),
//                    LicenseUrl = packageMetadata.LicenseUrl.ToEncodedUrlStringOrNull(),
//                    ProjectUrl = packageMetadata.ProjectUrl.ToEncodedUrlStringOrNull(),
//                    ReleaseNotes = packageMetadata.ReleaseNotes,
//                    RequiresLicenseAcceptance = packageMetadata.RequireLicenseAcceptance,
//                    Summary = packageMetadata.Summary,
//                    Tags = PackageHelper.ParseTags(packageMetadata.Tags),
//                    VersionTitle = packageMetadata.Title,
//                }
//            };
            return View(model);
        }

//        [Route("catalog/verify", HttpVerbs.Post)]
//        [ValidateAntiForgeryToken]
//        [ValidateInput(false)] // Security note: Disabling ASP.Net input validation which does things like disallow angle brackets in submissions. See http://go.microsoft.com/fwlink/?LinkID=212874
//        public virtual async Task<ActionResult> VerifyImport(VerifyPackageRequest formData)
//        {
//            var currentUser = GetCurrentUser();
//
//            Package package;
//            using (Stream uploadFile = await _uploadFileService.GetUploadFileAsync(currentUser.Key))
//            {
//                if (uploadFile == null)
//                {
//                    TempData["Message"] = "Your attempt to verify the package submission failed, because we could not find the uploaded package file. Please try again.";
//                    return new RedirectResult(Url.UploadPackage());
//                }
//
//                INupkg nugetPackage = CreatePackage(uploadFile);
//
//                // Rule out problem scenario with multiple tabs - verification request (possibly with edits) was submitted by user 
//                // viewing a different package to what was actually most recently uploaded
//                if (!(String.IsNullOrEmpty(formData.Id) || String.IsNullOrEmpty(formData.Version)))
//                {
//                    if (!(String.Equals(nugetPackage.Metadata.Id, formData.Id, StringComparison.OrdinalIgnoreCase)
//                        && String.Equals(nugetPackage.Metadata.Version.ToNormalizedString(), formData.Version, StringComparison.OrdinalIgnoreCase)))
//                    {
//                        TempData["Message"] = "Your attempt to verify the package submission failed, because the package file appears to have changed. Please try again.";
//                        return new RedirectResult(Url.VerifyPackage());
//                    }
//                }
//
//                bool pendEdit = false;
//                if (formData.Edit != null)
//                {
//                    pendEdit = pendEdit || formData.Edit.RequiresLicenseAcceptance != nugetPackage.Metadata.RequireLicenseAcceptance;
//
//                    pendEdit = pendEdit || IsDifferent(formData.Edit.IconUrl, nugetPackage.Metadata.IconUrl.ToEncodedUrlStringOrNull());
//                    pendEdit = pendEdit || IsDifferent(formData.Edit.ProjectUrl, nugetPackage.Metadata.ProjectUrl.ToEncodedUrlStringOrNull());
//
//                    pendEdit = pendEdit || IsDifferent(formData.Edit.Authors, nugetPackage.Metadata.Authors.Flatten());
//                    pendEdit = pendEdit || IsDifferent(formData.Edit.Copyright, nugetPackage.Metadata.Copyright);
//                    pendEdit = pendEdit || IsDifferent(formData.Edit.Description, nugetPackage.Metadata.Description);
//                    pendEdit = pendEdit || IsDifferent(formData.Edit.ReleaseNotes, nugetPackage.Metadata.ReleaseNotes);
//                    pendEdit = pendEdit || IsDifferent(formData.Edit.Summary, nugetPackage.Metadata.Summary);
//                    pendEdit = pendEdit || IsDifferent(formData.Edit.Tags, nugetPackage.Metadata.Tags);
//                    pendEdit = pendEdit || IsDifferent(formData.Edit.VersionTitle, nugetPackage.Metadata.Title);
//                }
//
//                // update relevant database tables
//                package = _packageService.CreatePackage(nugetPackage, currentUser, commitChanges: false);
//                Debug.Assert(package.PackageRegistration != null);
//
//                _packageService.PublishPackage(package, commitChanges: false);
//
//                if (pendEdit)
//                {
//                    // Add the edit request to a queue where it will be processed in the background.
//                    _editPackageService.StartEditPackageRequest(package, formData.Edit, currentUser);
//                }
//
//                if (!formData.Listed)
//                {
//                    _packageService.MarkPackageUnlisted(package, commitChanges: false);
//                }
//
//                _autoCuratedPackageCmd.Execute(package, nugetPackage, commitChanges: false);
//
//                // save package to blob storage
//                uploadFile.Position = 0;
//                await _packageFileService.SavePackageFileAsync(package, uploadFile);
//
//                // commit all changes to database as an atomic transaction
//                _entitiesContext.SaveChanges();
//
//                // tell Lucene to update index for the new package
//                _indexingService.UpdateIndex();
//
//                // If we're pushing a new stable version of NuGet.CommandLine, update the extracted executable.
//                if (package.PackageRegistration.Id.Equals(Constants.NuGetCommandLinePackageId, StringComparison.OrdinalIgnoreCase) &&
//                    package.IsLatestStable)
//                {
//                    await _nugetExeDownloaderService.UpdateExecutableAsync(nugetPackage);
//                }
//            }
//
//            // delete the uploaded binary in the Uploads container
//            await _uploadFileService.DeleteUploadFileAsync(currentUser.Key);
//
//            TempData["Message"] = String.Format(
//                CultureInfo.CurrentCulture, Strings.SuccessfullyUploadedPackage, package.PackageRegistration.Id, package.Version);
//
//            return RedirectToRoute(RouteName.DisplayPackage, new { package.PackageRegistration.Id, package.Version });
//        }

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