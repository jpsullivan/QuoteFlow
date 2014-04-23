using System.Text;

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

        /// <summary>
        /// Produces optional, URL-friendly version of a title, "like-this-one". 
        /// hand-tuned for speed, reflects performance refactoring contributed by John Gietzen (user otac0n) 
        /// </summary>
        public static string UrlFriendly(string title)
        {
            if (title == null) return "";

            const int maxlen = 80;
            int len = title.Length;
            bool prevdash = false;
            var sb = new StringBuilder(len);
            string s;
            char c;

            for (int i = 0; i < len; i++)
            {
                c = title[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                    prevdash = false;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    // tricky way to convert to lowercase
                    sb.Append((char)(c | 32));
                    prevdash = false;
                }
                else if (c == ' ' || c == ',' || c == '.' || c == '/' || c == '\\' || c == '-' || c == '_')
                {
                    if (!prevdash && sb.Length > 0)
                    {
                        sb.Append('-');
                        prevdash = true;
                    }
                }
                else if (c >= 128)
                {
                    s = c.ToString().ToLowerInvariant();
                    if ("àåáâäãåą".Contains(s))
                    {
                        sb.Append("a");
                    }
                    else if ("èéêëę".Contains(s))
                    {
                        sb.Append("e");
                    }
                    else if ("ìíîïı".Contains(s))
                    {
                        sb.Append("i");
                    }
                    else if ("òóôõöø".Contains(s))
                    {
                        sb.Append("o");
                    }
                    else if ("ùúûü".Contains(s))
                    {
                        sb.Append("u");
                    }
                    else if ("çćč".Contains(s))
                    {
                        sb.Append("c");
                    }
                    else if ("żźž".Contains(s))
                    {
                        sb.Append("z");
                    }
                    else if ("śşš".Contains(s))
                    {
                        sb.Append("s");
                    }
                    else if ("ñń".Contains(s))
                    {
                        sb.Append("n");
                    }
                    else if ("ýŸ".Contains(s))
                    {
                        sb.Append("y");
                    }
                    else if (c == 'ł')
                    {
                        sb.Append("l");
                    }
                    else if (c == 'đ')
                    {
                        sb.Append("d");
                    }
                    else if (c == 'ß')
                    {
                        sb.Append("ss");
                    }
                    else if (c == 'ğ')
                    {
                        sb.Append("g");
                    }
                    prevdash = false;
                }
                if (i == maxlen) break;
            }

            return prevdash ? sb.ToString().Substring(0, sb.Length - 1) : sb.ToString();
        }
    }
}