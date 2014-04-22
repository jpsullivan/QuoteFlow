using System;
using System.Text;
using System.Web;
using System.Web.Security;
using Jil;

namespace QuoteFlow.Infrastructure.Extensions
{
    public static class HttpContextExtensions
    {
        public static void SetConfirmationReturnUrl(this HttpContextBase httpContext, string returnUrl)
        {
            var confirmationContext = new ConfirmationContext
            {
                ReturnUrl = returnUrl,
            };
            string json = JSON.Serialize(confirmationContext);
            string protectedJson = Convert.ToBase64String(MachineKey.Protect(Encoding.UTF8.GetBytes(json), "ConfirmationContext"));
            httpContext.Response.Cookies.Add(new HttpCookie("ConfirmationContext", protectedJson));
        }

        public static string GetConfirmationReturnUrl(this HttpContextBase httpContext)
        {
            HttpCookie cookie = null;
            if (httpContext.Request.Cookies != null)
            {
                cookie = httpContext.Request.Cookies.Get("ConfirmationContext");
            }

            if (cookie == null)
            {
                return null;
            }

            var protectedJson = cookie.Value;
            if (String.IsNullOrEmpty(protectedJson))
            {
                return null;
            }

            string json = Encoding.UTF8.GetString(MachineKey.Unprotect(Convert.FromBase64String(protectedJson), "ConfirmationContext"));
            var confirmationContext = JSON.Deserialize<ConfirmationContext>(json);
            return confirmationContext.ReturnUrl;
        }
    }

    public class ConfirmationContext
    {
        public string ReturnUrl { get; set; }
    }
}