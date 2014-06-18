using System.Linq;
using System.Web.Mvc;
using QuoteFlow.Infrastructure.Attributes;
using QuoteFlow.Infrastructure.Extensions;
using QuoteFlow.Models.ViewModels;
using QuoteFlow.Models.ViewModels.Quotes;
using QuoteFlow.Services.Interfaces;
using Route = QuoteFlow.Infrastructure.Attributes.RouteAttribute;

namespace QuoteFlow.Controllers
{
    public class QuoteController : BaseController
    {
        #region IoC

        public IAssetService AssetService { get; protected set; }
        public IQuoteService QuoteService { get; protected set; }
        public IUserService UserService { get; protected set; }

        public QuoteController() { }

        public QuoteController(IAssetService assetService, 
            IQuoteService quoteService, 
            IUserService userService)
        {
            AssetService = assetService;
            QuoteService = quoteService;
            UserService = userService;
        }

        #endregion

        [Route("quotes")]
        public ActionResult Index()
        {
            var quotes = QuoteService.GetQuotesFromOrganization(CurrentOrganization.Id);

            return View(quotes);
        }

        [Route("quote/new")]
        public ActionResult New()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [Route("quote/create", HttpVerbs.Post)]
        public ActionResult CreateQuote(NewQuoteModel model)
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

        [Route("quote/{id:INT}/{name}")]
        public ActionResult Show(int id, string name)
        {
            var quote = QuoteService.GetQuote(id);
            if (quote == null)
            {
                return PageNotFound();
            }

            var creator = UserService.GetUser(quote.CreatorId);

            var model = new QuoteShowModel
            {
                Quote = quote,
                QuoteCreator = creator,
                CurrentAsset = new AssetDetailsModel(AssetService.GetAsset(1), true)
            };

            return quote.Name.UrlFriendly() != name ? PageNotFound() : View(model);
        }
    }
}
