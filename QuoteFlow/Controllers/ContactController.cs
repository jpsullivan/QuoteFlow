using System.Web.Mvc;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Models.ViewModels;
using QuoteFlow.Api.Models.ViewModels.Contacts;
using QuoteFlow.Api.Services;
using QuoteFlow.Infrastructure;
using QuoteFlow.Infrastructure.Attributes;
using QuoteFlow.Infrastructure.Extensions;

namespace QuoteFlow.Controllers
{
    public class ContactController : AppController
    {
        #region DI

        public IContactService ContactService { get; protected set; }

        public ContactController()
        {
        }

        public ContactController(IContactService contactService)
        {
            ContactService = contactService;
        }

        #endregion

        [QuoteFlowRoute("contacts", Name = RouteNames.Contacts)]
        public virtual ActionResult Index()
        {
            var contacts = ContactService.GetContactsFromOrganization(CurrentOrganization.Id);
            var model = new ContactsViewModel(contacts);
            return View(model);
        }

        [QuoteFlowRoute("contact/new", HttpVerbs.Get)]
        [LayoutInjector("_LayoutWorkflow")]
        public virtual ActionResult New()
        {
            return View();
        }

        [QuoteFlowRoute("contact/create", HttpVerbs.Post)]
        public virtual ActionResult CreateContact(NewContactModel model)
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

        [QuoteFlowRoute("contact/{contactId}/{contactNameSlug}")]
        public virtual ActionResult Show(int contactId, string contactNameSlug)
        {
            var contact = ContactService.GetContact(contactId);
            return View(contact);
        }
    }
}
