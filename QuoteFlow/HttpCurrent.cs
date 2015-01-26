using System.Web;
using QuoteFlow.Controllers;

namespace QuoteFlow
{
    public class HttpCurrent
    {
        /// <summary>
        /// Shortcut to HttpContext.Current.
        /// </summary>
        public static HttpContext Context
        {
            get { return HttpContext.Current; }
        }

        /// <summary>
        /// Gets the controller for the current request; should be set during init of current request's controller.
        /// </summary>
        public AppController Controller
        {
            get { return Context.Items["Controller"] as AppController; }
            set { Context.Items["Controller"] = value; }
        }
    }
}