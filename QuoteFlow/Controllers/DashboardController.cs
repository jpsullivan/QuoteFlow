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

        public IUserService UserService { get; protected set; }

        public DashboardController(IUserService userService)
        {
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
            model.Catalogs = catalogs;

            return View(model);
        }
    }
}