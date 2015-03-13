using System.Web.Mvc;

namespace QuoteFlow.Controllers
{
    public class ErrorsController : AppController
    {
        [Infrastructure.Attributes.Route("error/404")]
        public ActionResult Error404()
        {
            return View();
        }
    }
}