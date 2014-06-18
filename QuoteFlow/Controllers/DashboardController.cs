using System.Linq;
using System.Web.Mvc;
using QuoteFlow.Infrastructure.Attributes;
using QuoteFlow.Models.ViewModels;
using QuoteFlow.Services.Interfaces;
using Route = QuoteFlow.Infrastructure.Attributes.RouteAttribute;

namespace QuoteFlow.Controllers
{
    public partial class DashboardController : BaseController
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

        [Route("")]
        public virtual ActionResult Index()
        {
            var model = new DashboardViewModel();

            if (!Request.IsAuthenticated) {
                return View(model);
            }

            var currentUser = GetCurrentUser();
            var catalogs = UserService.GetCatalogs(currentUser.Id);
            var quotes = QuoteService.GetQuotesFromOrganization(currentUser.Organizations.First().Id);
            model.Catalogs = catalogs;
            model.Quotes = quotes;

            return View(model);
        }
    }
}