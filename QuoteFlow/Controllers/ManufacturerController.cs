using System;
using System.Linq;
using System.Web.Mvc;
using Dapper;
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
        
        public IManufacturerService ManufacturerService { get; set; }

        public ManufacturerController() { }

        public ManufacturerController(IManufacturerService manufacturerService)
        {
            ManufacturerService = manufacturerService;
        }

        #endregion

        [Route("manufacturer/{id:INT}/{name}", Name = "Manufacturer-Show")]
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

        [Route("manufacturer/{id:INT}/{manufacturerName}/edit", HttpVerbs.Get)]
        public ActionResult Edit(int id, string manufacturerName)
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
        public ActionResult Edit(int id, string manufacturerName, EditManufacturerRequest form, string returnUrl)
        {
            var manufacturer = ManufacturerService.GetManufacturer(id);
            if (manufacturer == null)
            {
                return HttpNotFound();
            }

            // ensure that the provided asset matches the expected asset
            if (manufacturer.Id != id)
                return SafeRedirect(returnUrl ?? Url.Manufacturer(id, manufacturerName));

            if (!ModelState.IsValid)
            {
                // todo: show some kind of form validation error
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

            return SafeRedirect(returnUrl ?? Url.Manufacturer(id, manufacturerName));
        }
    }
}