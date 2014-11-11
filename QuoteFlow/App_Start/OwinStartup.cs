using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Routing;
using Elmah.Contrib.WebApi;
using Ninject.Web.Common.OwinHost;
using Ninject.Web.WebApi.OwinHost;
using Owin;
using Ninject;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using QuoteFlow.Authentication;
using QuoteFlow.Authentication.Providers;
using QuoteFlow.Authentication.Providers.LocalUser;
using QuoteFlow.Configuration;
using QuoteFlow.Infrastructure;

[assembly: OwinStartup(typeof(QuoteFlow.OwinStartup))]

namespace QuoteFlow
{
    public class OwinStartup
    {
        // This method is auto-detected by the OWIN pipeline. DO NOT RENAME IT!
        public static void Configuration(IAppBuilder app)
        {
            // Configure Ninject
            NinjectSetup(app);

            // Get config
            var config = Container.Kernel.Get<ConfigurationService>();
            var auth = Container.Kernel.Get<AuthenticationService>();

            // Configure logging
            app.SetLoggerFactory(new DiagnosticsLoggerFactory());

            if (config.Current.RequireSSL)
            {
                // Put a middleware at the top of the stack to force the user over to SSL
                // if authenticated.
                app.UseForceSslWhenAuthenticated(config.Current.SSLPort);
            }

            // Configure auth
            LoadAuth(config, auth, app);

            // Configure WebApi
            WebApiSetup(app);
        }

        private static void NinjectSetup(IAppBuilder app)
        {
            app.UseNinjectMiddleware(() => Container.Kernel);
        }

        private static void WebApiSetup(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            // Incorporate extra Get methods while still maintaining original WebAPI REST functionality
            config.Routes.MapHttpRoute("DefaultApiWithId", "Api/{controller}/{id}", new { id = RouteParameter.Optional }, new { id = @"\d+" });
            config.Routes.MapHttpRoute("DefaultApiWithAction", "Api/{controller}/{action}");
            config.Routes.MapHttpRoute("DefaultApiGet", "Api/{controller}", new { action = "Get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("DefaultApiPost", "Api/{controller}", new { action = "Post" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("ApiWithDoubleParamDelete", "Api/{controller}/{id}/{secondaryId}", new { action = "Delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });

            // only use json for webapi output
            var jsonFormatter = new JsonMediaTypeFormatter();
            config.Services.Replace(typeof(IContentNegotiator), new JsonContentNegotiator(jsonFormatter));

            // enable elmah
            config.Services.Add(typeof(IExceptionLogger), new ElmahExceptionLogger());

            // internally calls app.UseWebApi
            app.UseNinjectWebApi(config);
        }

        private static void LoadAuth(ConfigurationService config, AuthenticationService auth, IAppBuilder app)
        {
            // Get the local user auth provider, if present and attach it first
            Authenticator localUserAuther;
            if (auth.Authenticators.TryGetValue(Authenticator.GetName(typeof(LocalUserAuthenticator)), out localUserAuther))
            {
                // Configure cookie auth now
                localUserAuther.Startup(config, app);
            }

            // Attach external sign-in cookie middleware
            app.SetDefaultSignInAsAuthenticationType(AuthenticationTypes.External);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = AuthenticationTypes.External,
                AuthenticationMode = AuthenticationMode.Passive,
                CookieName = ".AspNet." + AuthenticationTypes.External,
                ExpireTimeSpan = TimeSpan.FromMinutes(5)
            });

            // Attach non-cookie auth providers
            var nonCookieAuthers = auth
                .Authenticators
                .Where(p => !String.Equals(
                    p.Key,
                    Authenticator.GetName(typeof(LocalUserAuthenticator)),
                    StringComparison.OrdinalIgnoreCase))
                .Select(p => p.Value);

            foreach (var auther in nonCookieAuthers)
            {
                auther.Startup(config, app);
            }
        }
    }
}