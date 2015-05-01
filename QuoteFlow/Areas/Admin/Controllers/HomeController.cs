using System.Web.Mvc;
using QuoteFlow.Infrastructure.Attributes;

namespace QuoteFlow.Areas.Admin.Controllers
{
    public class HomeController : AdminControllerBase
    {
        [QuoteFlowRoute("admin")]
        public ActionResult Index()
        {
            return View();
        }
    }
}