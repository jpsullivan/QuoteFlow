using System.Web.Mvc;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Models.ViewModels;
using QuoteFlow.Api.Models.ViewModels.Customers;
using QuoteFlow.Api.Services;
using QuoteFlow.Infrastructure;
using QuoteFlow.Infrastructure.Attributes;
using QuoteFlow.Infrastructure.Extensions;

namespace QuoteFlow.Controllers
{
    public class CustomerController : AppController
    {
        #region DI

        public ICustomerService CustomerService { get; protected set; }

        public CustomerController()
        {
        }

        public CustomerController(ICustomerService customerService)
        {
            CustomerService = customerService;
        }

        #endregion

        [QuoteFlowRoute("customers", Name = RouteNames.Customers)]
        public virtual ActionResult Index()
        {
            var contacts = CustomerService.GetCustomersFromOrganization(CurrentOrganization.Id);
            var model = new CustomersViewModel(contacts);
            return View(model);
        }

        [QuoteFlowRoute("customer/new", HttpVerbs.Get, Name = RouteNames.CustomerNew)]
        public virtual ActionResult New()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [QuoteFlowRoute("customer/create", HttpVerbs.Post)]
        public ActionResult CreateContact(NewContactModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("New", model);
            }

            var contact = new Customer
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

            var newContact = CustomerService.CreateCustomer(contact);
            
            return SafeRedirect(Url.Customer(newContact.Id, newContact.FullName.UrlFriendly()));
        }

        [QuoteFlowRoute("customer/{id:INT}/{name}", Name = RouteNames.CustomerShow)]
        public virtual ActionResult Show(int id, string name)
        {
            var customer = CustomerService.GetCustomer(id);
            var model = new CustomerShowModel(customer);
            return View(model);
        }
    }
}
