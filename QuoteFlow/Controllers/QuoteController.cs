using System.Linq;
using System.Web.Mvc;
using QuoteFlow.Api.Models.ViewModels.Quotes;
using QuoteFlow.Api.Services;
using QuoteFlow.Infrastructure;
using QuoteFlow.Infrastructure.Attributes;
using QuoteFlow.Infrastructure.Extensions;
using Route = QuoteFlow.Infrastructure.Attributes.RouteAttribute;

namespace QuoteFlow.Controllers
{
    public class QuoteController : AppController
    {
        #region DI

        public IAssetService AssetService { get; protected set; }
        public IQuoteService QuoteService { get; protected set; }
        public IQuoteStatusService QuoteStatusService { get; protected set; }
        public IUserService UserService { get; protected set; }

        public QuoteController() { }

        public QuoteController(IAssetService assetService, 
            IQuoteService quoteService, 
            IQuoteStatusService quoteStatusService,
            IUserService userService)
        {
            AssetService = assetService;
            QuoteService = quoteService;
            QuoteStatusService = quoteStatusService;
            UserService = userService;
        }

        #endregion

        [Route("quotes", Name = RouteNames.QuoteIndex)]
        public virtual ActionResult Index()
        {
            var quotes = QuoteService.GetQuotesFromOrganization(CurrentOrganization.Id);
            var model = new QuotesViewModel(quotes);
            return View(model);
        }

        [Route("quote/new")]
        public virtual ActionResult New()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [Route("quote/create", HttpVerbs.Post)]
        public virtual ActionResult CreateQuote(NewQuoteModel model)
        {
            var currentUser = GetCurrentUser();

            // lazily set the organization to the first one that the user is assigned to.
            // eventually use whatever the users' current organization is once multi-tenancy
            // is implemented.
            model.Organization = currentUser.Organizations.First();

            if (!ModelState.IsValid) return New();

            // check to make sure that a quote with this name doesn't already exist
            if (QuoteService.ExistsWithinOrganization(model.QuoteName, CurrentOrganization.Id)) {
                var errorMsg = string.Format("Quote name already exists within the {0} organization.",
                    model.Organization.OrganizationName);
                ModelState.AddModelError("Name", errorMsg);
                return View("New", model);
            }

            var newQuote = QuoteService.CreateQuote(model, currentUser.Id);

            // there has to be a better way to do this...
            return Redirect("~/quote/" + newQuote.Id + "/" + newQuote.Name.UrlFriendly());
        }

        [Route("quote/{id:INT}/{name}", Name = RouteNames.QuoteShow)]
        public virtual ActionResult Show(int id, string name)
        {
            var quote = QuoteService.GetQuote(id);
            if (quote == null)
            {
                return PageNotFound();
            }

            var creator = UserService.GetUser(quote.CreatorId);
            var statuses = QuoteStatusService.GetStatuses(CurrentOrganization.Id);

            var model = new QuoteShowModel(quote, creator, statuses);
            return quote.Name.UrlFriendly() != name ? PageNotFound() : View(model);
        }

        [Route("quote/{id:INT}/{name}/line-items", Name = RouteNames.QuoteLineItems)]
        public virtual ActionResult LineItems(int id, string name)
        {
            var quote = QuoteService.GetQuote(id);
            if (quote == null)
            {
                return PageNotFound();
            }

            var creator = UserService.GetUser(quote.CreatorId);
            var model = new QuoteShowModel(quote, creator);
            return quote.Name.UrlFriendly() != name ? PageNotFound() : View(model);
        }

        [Route("quote/{id:INT}/{name}/change-history", Name = RouteNames.QuoteChangeHistory)]
        public virtual ActionResult ChangeHistory(int id, string name)
        {
            var quote = QuoteService.GetQuote(id);
            if (quote == null)
            {
                return PageNotFound();
            }

            var creator = UserService.GetUser(quote.CreatorId);

            var model = new QuoteShowModel(quote, creator);
            return quote.Name.UrlFriendly() != name ? PageNotFound() : View(model);
        }

        [Route("quote/{id:INT}/{name}/access", Name = RouteNames.QuoteAccessControl)]
        public virtual ActionResult AccessControl(int id, string name)
        {
            var quote = QuoteService.GetQuote(id);
            if (quote == null)
            {
                return PageNotFound();
            }

            var creator = UserService.GetUser(quote.CreatorId);

            var model = new QuoteShowModel(quote, creator);
            return quote.Name.UrlFriendly() != name ? PageNotFound() : View(model);
        }
    }
}
