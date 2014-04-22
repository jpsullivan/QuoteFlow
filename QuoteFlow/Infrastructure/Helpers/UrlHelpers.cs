namespace System.Web.Mvc
{
    public static class UrlHelpers
    {
        public static string SiteRoot(HttpContextBase context, bool usePort = true)
        {
            var port = context.Request.ServerVariables["SERVER_PORT"];
            if (usePort)
            {
                if (port == null || port == "80" || port == "443")
                {
                    port = "";
                }
                else
                {
                    port = ":" + port;
                }
            }
            var protocol = context.Request.ServerVariables["SERVER_PORT_SECURE"];
            if (protocol == null || protocol == "0")
            {
                protocol = "http://";
            }
            else
            {
                protocol = "https://";
            }
            var appPath = context.Request.ApplicationPath;
            if (appPath == "/")
            {
                appPath = "";
            }

            var sOut = protocol + context.Request.ServerVariables["SERVER_NAME"] + port + appPath;
            return sOut;
        }

        public static string SiteRoot(this UrlHelper url)
        {
            return SiteRoot(url.RequestContext.HttpContext);
        }

        public static string SiteRoot(this ViewPage pg)
        {
            return SiteRoot(pg.ViewContext.HttpContext);
        }

        public static string SiteRoot(this ViewUserControl pg)
        {
            var vpage = pg.Page as ViewPage;
            return SiteRoot(vpage.ViewContext.HttpContext);
        }

        public static string SiteRoot(this ViewMasterPage pg)
        {
            return SiteRoot(pg.ViewContext.HttpContext);
        }

        public static string GetReturnUrl(HttpContextBase context)
        {
            var returnUrl = "";

            if (context.Request.QueryString["ReturnUrl"] != null)
            {
                returnUrl = context.Request.QueryString["ReturnUrl"];
            }

            return returnUrl;
        }

        public static string GetReturnUrl(this UrlHelper helper)
        {
            return GetReturnUrl(helper.RequestContext.HttpContext);
        }

        public static string GetReturnUrl(this ViewPage pg)
        {
            return GetReturnUrl(pg.ViewContext.HttpContext);
        }

        public static string GetReturnUrl(this ViewMasterPage pg)
        {
            return GetReturnUrl(pg.Page as ViewPage);
        }

        public static string GetReturnUrl(this ViewUserControl pg)
        {
            return GetReturnUrl(pg.Page as ViewPage);
        }
    }
}