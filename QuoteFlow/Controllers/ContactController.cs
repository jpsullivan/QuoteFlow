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

        [QuoteFlowRoute("contact/new", HttpVerbs.Get, Name = RouteNames.ContactNew)]
        public virtual ActionResult New()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [QuoteFlowRoute("contact/create", HttpVerbs.Post)]
        public ActionResult CreateContact(NewContactModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("New", model);
            }

            var contact = new Contact
            {
                OrganizationId = CurrentOrganization.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailAddress = model.Email,
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
            
            return SafeRedirect(Url.Contact(newContact.Id, newContact.FullName.UrlFriendly()));
        }

        [QuoteFlowRoute("contact/{id:INT}/{name}", Name = RouteNames.ContactShow)]
        public virtual ActionResult Show(int id, string name)
        {
            var contact = ContactService.GetContact(id);
            return View(contact);
        }
    }
}
