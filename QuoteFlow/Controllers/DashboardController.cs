using System.Web.Mvc;
using QuoteFlow.Infrastructure.Attributes;
using Route = QuoteFlow.Infrastructure.Attributes.RouteAttribute;

namespace QuoteFlow.Controllers
{
    public partial class DashboardController : BaseController
    {
        [Route("")]
        public virtual ActionResult Index()
        {
            return View();
        }
    }
}