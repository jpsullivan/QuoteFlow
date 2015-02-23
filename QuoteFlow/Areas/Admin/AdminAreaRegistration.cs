using System.Web.Mvc;

namespace QuoteFlow.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "Admin"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.Routes.Ignore("Admin/Errors.axd/{*pathInfo}"); // ELMAH owns this root
            context.Routes.Ignore("Admin/Glimpse/{*pathInfo}"); // Glimpse owns this root

            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            // enforce lower-case URL's so that case-sentive routes don't break
            context.Routes.LowercaseUrls = true;
        }
    }
}