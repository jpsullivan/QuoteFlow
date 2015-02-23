using System.Web.Mvc;

namespace QuoteFlow.Areas.Admin.Controllers
{
    public class HomeController : AdminControllerBase
    {
        [Infrastructure.Attributes.Route("admin")]
        public ActionResult Index()
        {
            return View();
        }
    }
}