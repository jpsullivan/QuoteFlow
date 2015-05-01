using System.Net;
using System.Web.Mvc;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Controllers
{
    public class ErrorController : AppController
    {
        /// <summary>
        /// Returns an HTTP 404 Not Found error view. Returns a partial view if the request 
        /// is an AJAX call.
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "NotFound")]
        [Infrastructure.Attributes.QuoteFlowRoute("error/notfound")]
        public ActionResult NotFound()
        {
            return GetErrorView(HttpStatusCode.NotFound, "NotFound");
        }

        private ActionResult GetErrorView(HttpStatusCode statusCode, string viewName)
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;

            // Don't show IIS custom errors.
            Response.TrySkipIisCustomErrors = true;

            PageErrorModel error = new PageErrorModel()
            {
                RequestedUrl = Request.Url.ToString(),
                ReferrerUrl =
                    (Request.UrlReferrer == null) 
                        ? null 
                        : Request.UrlReferrer.ToString()
            };

            ActionResult result;
            if (Request.IsAjaxRequest())
            {
                // This allows us to show not found errors even in partial views.
                result = PartialView(viewName, error);
            }
            else
            {
                result = View(viewName, error);
            }

            return result;
        }
    }
}