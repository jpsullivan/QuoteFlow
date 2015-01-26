using System.Linq;
using System.Web.Mvc;
using QuoteFlow.Api.Services;
using QuoteFlow.Infrastructure.Attributes;
using QuoteFlow.Models.ViewModels;
using Route = QuoteFlow.Infrastructure.Attributes.RouteAttribute;

namespace QuoteFlow.Controllers
{
    public partial class DashboardController : AppController
    {
        #region Ioc

        public IQuoteService QuoteService { get; protected set; }
        public IUserService UserService { get; protected set; }

        public DashboardController(IQuoteService quoteService, IUserService userService)
        {
            QuoteService = quoteService;
            UserService = userService;
        }

        #endregion

        [Authorize]
        [Route("")]
        public virtual ActionResult Index(int? skipGettingStarted)
        {
            var model = new DashboardViewModel();

            model.SkipGettingStarted = true;

            if (!Request.IsAuthenticated)
            {
                return View(model);
            }

            var currentUser = GetCurrentUser();
            var catalogs = UserService.GetCatalogs(currentUser.Id).ToList();
            var quotes = QuoteService.GetQuotesFromOrganization(currentUser.Organizations.First().Id).ToList();

            // if there aren't any catalogs or quotes, show the getting started screen.
            if (!catalogs.Any() && !quotes.Any())
            {
                model.SkipGettingStarted = false;
            }

            model.Catalogs = catalogs;
            model.Quotes = quotes;

            // catch-all, if the query string is forcing a skip, do so.
            if (skipGettingStarted != null)
            {
                if (skipGettingStarted == 1)
                {
                    model.SkipGettingStarted = true;
                }
            }

            return View(model);
        }

        [Route("getting-started")]
        public virtual ActionResult GettingStarted()
        {
            return View();
        }
    }
}