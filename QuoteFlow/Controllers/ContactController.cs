using System.Web.Mvc;
using QuoteFlow.Infrastructure.Attributes;
using QuoteFlow.Infrastructure.Extensions;
using QuoteFlow.Models;
using QuoteFlow.Models.ViewModels;
using QuoteFlow.Services.Interfaces;
using Route = QuoteFlow.Infrastructure.Attributes.RouteAttribute;

namespace QuoteFlow.Controllers
{
    public class ContactController : BaseController
    {
        #region IoC

        public IContactService ContactService { get; protected set; }

        public ContactController() { }

        public ContactController(IContactService contactService)
        {
            ContactService = contactService;
        }

        #endregion

        [Route("contacts")]
        public ActionResult Index()
        {
            var contacts = ContactService.GetContactsByOrganizationId(CurrentOrganization.Id);
            return View(contacts);
        }

        [Route("contact/new", HttpVerbs.Get)]
        [LayoutInjector("_LayoutWorkflow")]
        public ActionResult New()
        {
            return View();
        }

        [Route("contact/create", HttpVerbs.Post)]
        public ActionResult CreateContact(NewContactModel model)
        {
            if (ContactService.ContactExists(model.FirstName, model.LastName, model.Email, CurrentOrganization.Id))
            {
                // TODO: Return saying that the org name already exists
                return RedirectToAction("New", "Contact");
            }

            // TODO form validation

            var contact = new Contact
            {
                OrganizationId = CurrentOrganization.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Address1 = model.Address1,
                Address2 = model.Address2,
                Country = model.Country,
                State = model.State,
                City = model.City,
                Phone = model.PhoneNumber,
                Organization = model.Organization,
                Title = model.Title,
                Zipcode = model.Zipcode
            };

            var newContact = ContactService.CreateContact(contact);

            // there has to be a better way to do this...
            return Redirect("~/contact/" + newContact.Id + "/" + newContact.FullName.UrlFriendly());
        }

        [Route("contact/{contactId}/{contactNameSlug}")]
        public ActionResult Show(int contactId, string contactNameSlug)
        {
            var contact = ContactService.GetContact(contactId);
            return View(contact);
        }
    }
}
