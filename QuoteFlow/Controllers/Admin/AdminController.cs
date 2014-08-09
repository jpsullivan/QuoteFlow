using System.Web.Mvc;
using QuoteFlow.Infrastructure.Attributes;
using Route = QuoteFlow.Infrastructure.Attributes.RouteAttribute;

namespace QuoteFlow.Controllers.Admin
{
    public partial class AdminController : AdminControllerBase
    {
        [Route("admin")]
        public virtual ActionResult AdminIndex()
        {
            return View();
        }
    }
}