using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Dapper;
using QuoteFlow.Infrastructure.Attributes;
using QuoteFlow.Infrastructure.Extensions;
using QuoteFlow.Models.ViewModels.Manufacturers;
using QuoteFlow.Services;
using QuoteFlow.Services.Interfaces;
using Route = QuoteFlow.Infrastructure.Attributes.RouteAttribute;

namespace QuoteFlow.Controllers
{
    public partial class ManufacturerController : AppController
    {
        #region IoC
        
        public IManufacturerService ManufacturerService { get; set; }
        public IUploadFileService UploadFileService { get; protected set; }

        public ManufacturerController() { }

        public ManufacturerController(
            IManufacturerService manufacturerService,
            IUploadFileService uploadFileService
            )
        {
            ManufacturerService = manufacturerService;
            UploadFileService = uploadFileService;
        }

        #endregion

        [Route("manufacturer/{id:INT}/{name}", Name = "Manufacturer-Show")]
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

        [Route("manufacturer/{id:INT}/{manufacturerName}/edit", HttpVerbs.Get)]
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

        [Route("manufacturer/{id:INT}/{manufacturerName}/edit", HttpVerbs.Post)]
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
                    var filename = string.Format("{0}_{1}{2}", manufacturer.Id, manufacturer.Name.UrlFriendly(), extension);
                    await UploadFileService.SaveUploadFileAsync(GetCurrentUser().Id, uploadStream, extension, UploadType.ManufacturerLogo, filename);
                }
            }

            return SafeRedirect(returnUrl ?? Url.Manufacturer(id, manufacturerName));
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