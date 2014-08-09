using System.Web.Mvc;
using QuoteFlow.Infrastructure.Attributes;
using Route = QuoteFlow.Infrastructure.Attributes.RouteAttribute;

namespace QuoteFlow.Controllers.Admin
{
    public class AdminController : AdminControllerBase
    {
        [Route("admin")]
        public ActionResult AdminIndex()
        {
            return View();
        }
    }
}