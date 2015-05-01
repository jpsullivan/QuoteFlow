using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Dapper;
using QuoteFlow.Api.Models.ViewModels.Manufacturers;
using QuoteFlow.Api.Services;
using QuoteFlow.Api.Upload;
using QuoteFlow.Infrastructure.Attributes;
using QuoteFlow.Infrastructure.Extensions;

namespace QuoteFlow.Controllers
{
    public partial class ManufacturerController : AppController
    {
        #region IoC
        
        public IManufacturerService ManufacturerService { get; set; }
        public IManufacturerLogoService ManufacturerLogoService { get; set; }
        public IUploadFileService UploadFileService { get; protected set; }

        public ManufacturerController() { }

        public ManufacturerController(
            IManufacturerService manufacturerService,
            IManufacturerLogoService manufacturerLogoService,
            IUploadFileService uploadFileService
            )
        {
            ManufacturerService = manufacturerService;
            ManufacturerLogoService = manufacturerLogoService;
            UploadFileService = uploadFileService;
        }

        #endregion

        [QuoteFlowRoute("manufacturer/{id:INT}/{name}", Name = "Manufacturer-Show")]
        public virtual ActionResult Show(int id, string name)
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

        [QuoteFlowRoute("manufacturer/{id:INT}/{manufacturerName}/edit", HttpVerbs.Get)]
        public virtual ActionResult Edit(int id, string manufacturerName)
        {
            var manufacturer = ManufacturerService.GetManufacturer(id);
            if (manufacturer == null)
            {
                return HttpNotFound();
            }

            var viewModel = new EditManufacturerRequest
            {
                Id = manufacturer.Id,
                Name = manufacturer.Name,
                Description = manufacturer.Description
            };

            return manufacturer.Name.UrlFriendly() != manufacturerName? PageNotFound() : View(viewModel);
        }

        [QuoteFlowRoute("manufacturer/{id:INT}/{manufacturerName}/edit", HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Edit(int id, string manufacturerName, EditManufacturerRequest form, string returnUrl)
        {
            var manufacturer = ManufacturerService.GetManufacturer(id);
            if (manufacturer == null)
            {
                return HttpNotFound();
            }

            // ensure that the provided asset matches the expected asset
            if (manufacturer.Id != id)
            {
                return SafeRedirect(returnUrl ?? Url.Manufacturer(id, manufacturerName));
            }

            // if a logo exists and it doesn't pass the file extension check, bomb out
            if (form.ManufacturerLogo != null && !VerifyManufacturerLogoExtension(form.ManufacturerLogo))
            {
                ModelState.AddModelError("ManufacturerLogo", "Please upload either a GIF, JPG or PNG image.");
            }

            if (!ModelState.IsValid)
            {
                return View(form);
            }

            var snapshot = Snapshotter.Start(manufacturer);
            manufacturer.Name = form.Name;
            manufacturer.Description = form.Description;
            manufacturer.LastUpdated = DateTime.UtcNow;

            var diff = snapshot.Diff();
            if (diff.ParameterNames.Any())
            {
                ManufacturerService.UpdateManufacturer(manufacturer.Id, diff);
            }

            if (form.ManufacturerLogo != null)
            {
                using (var uploadStream = form.ManufacturerLogo.InputStream)
                {
                    var extension = Path.GetExtension(form.ManufacturerLogo.FileName);
                    var filename = string.Format("{0}-{1}{2}", manufacturer.Id.ToString(), Guid.NewGuid().ToString(), extension);
                    await UploadFileService.SaveUploadFileAsync(GetCurrentUser().Id, uploadStream, extension, UploadType.ManufacturerLogo, filename);
                    ManufacturerLogoService.CreateManufacturerLogo(manufacturer.Id, Guid.NewGuid(), filename);
                }
            }

            return SafeRedirect(returnUrl ?? Url.Manufacturer(id, manufacturerName));
        }

        [QuoteFlowRoute("manufacturer/{id:INT}/{manufacturerName}/logo", HttpVerbs.Get, RoutePriority.Default, RouteHandler.ManufacturerLogo)]
        public void GetManufacturerLogo(int id, string manufacturerName)
        {
        }

        /// <summary>
        /// Verifies that the uploaded manufacturer logo is of an
        /// accepted file extension.
        /// </summary>
        /// <param name="file">The uploaded manufacturer logo file</param>
        /// <returns></returns>
        [NonAction]
        private bool VerifyManufacturerLogoExtension(HttpPostedFileBase file)
        {
            var validImageTypes = new[]
            {
                "image/gif",
                "image/jpeg",
                "image/pjpeg",
                "image/png"
            };

            return file.ContentLength <= 0 || validImageTypes.Contains(file.ContentType);
        }
    }
}