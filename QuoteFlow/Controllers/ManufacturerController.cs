using System.Web.Mvc;
using QuoteFlow.Infrastructure.Attributes;
using QuoteFlow.Infrastructure.Extensions;
using QuoteFlow.Models.ViewModels.Manufacturers;
using QuoteFlow.Services.Interfaces;
using Route = QuoteFlow.Infrastructure.Attributes.RouteAttribute;

namespace QuoteFlow.Controllers
{
    public class ManufacturerController : BaseController
    {
        #region IoC

        public IAssetService AssetService { get; protected set; }
        public ICatalogService CatalogService { get; protected set; }
        public ICatalogImportSummaryRecordsService CatalogImportSummaryService { get; set; }
        public IManufacturerService ManufacturerService { get; set; }
        public IOrganizationService OrganizationService { get; protected set; }
        public IUserService UserService { get; protected set; }
        public IUploadFileService UploadFileService { get; protected set; }
        public ICacheService CacheService { get; protected set; }

        public ManufacturerController() { }

        public ManufacturerController(IAssetService assetService, 
            ICatalogService catalogService, 
            ICatalogImportSummaryRecordsService catalogImportSummaryService,
            IManufacturerService manufacturerService,
            IOrganizationService organizationService, 
            IUserService userService,
            IUploadFileService uploadFileService, 
            ICacheService cacheService)
        {
            AssetService = assetService;
            CatalogService = catalogService;
            CatalogImportSummaryService = catalogImportSummaryService;
            ManufacturerService = manufacturerService;
            OrganizationService = organizationService;
            UserService = userService;
            UploadFileService = uploadFileService;
            CacheService = cacheService;
        }

        #endregion

        [Route("manufacturer/{id:INT}/{name}")]
        public ActionResult Show(int id, string name)
        {
            var manufacturer = ManufacturerService.GetManufacturer(id);
            if (manufacturer == null)
            {
                return PageNotFound();
            }

            var model = new ManufacturerShowModel
            {
                Manufacturer = manufacturer
            };

            return manufacturer.Name.UrlFriendly() != name ? PageNotFound() : View(model);
        }

        [Route("manufacturer/{id:INT}/{name}/edit", HttpVerbs.Get)]
        public ActionResult Edit()
        {
            return new EmptyResult();
        }
    }
}