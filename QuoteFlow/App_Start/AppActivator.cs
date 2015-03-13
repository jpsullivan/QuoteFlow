using System.Security.Claims;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Elmah;
using Elmah.Contrib.Mvc;
using Elmah.Contrib.WebApi;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject.Web.Common;
using QuoteFlow;
using QuoteFlow.Api.Models.ViewModels.Assets;
using QuoteFlow.Core.DependencyResolution;
using QuoteFlow.Infrastructure.Attributes;
using QuoteFlow.Infrastructure.ModelBinders.Asset;
using WebActivatorEx;
using RouteAttribute = QuoteFlow.Infrastructure.Attributes.RouteAttribute;

[assembly: PreApplicationStartMethod(typeof(AppActivator), "PreStart")]
[assembly: PostApplicationStartMethod(typeof(AppActivator), "PostStart")]
[assembly: ApplicationShutdownMethod(typeof(AppActivator), "Stop")]

namespace QuoteFlow
{
    public static class AppActivator
    {
        private static readonly Bootstrapper NinjectBootstrapper = new Bootstrapper();

        public static void PreStart()
        {
            // reset all viewengines except razor for perf purposes
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

            //NinjectPreStart();
            ElmahPreStart();
            GlimpsePreStart();
        }

        public static void PostStart()
        {
            AppPostStart();
        }

        public static void Stop()
        {
            NinjectStop();
        }

        private static void GlimpsePreStart()
        {
            //DynamicModuleUtility.RegisterModule(typeof(Glimpse.AspNet.HttpModule));
        }

        private static void ElmahPreStart()
        {
            ServiceCenter.Current = _ => Container.Kernel;
        }

        private static void AppPostStart()
        {
            // disable the X-AspNetMvc-Version: header
            MvcHandler.DisableMvcResponseHeader = true;

            RegisterAllRouters();
            AddGlobalFilters();
            AddModelBinders();
        }

        /// <summary>
        /// Initializes and registers all of the routers that are used within
        /// the application. This includes Web Api, SignalR, and the standard MVC routes.
        /// </summary>
        private static void RegisterAllRouters()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            // Set up MVC routes so our app URLs actually work
            // IMPORTANT: This must be called last as far as routing goes
            RegisterRoutes(RouteTable.Routes);
        }

        /// <summary>
        /// Register our ASP.NET MVC routes
        /// </summary>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("errors");
            routes.IgnoreRoute("errors/{*pathInfo}");
            //routes.IgnoreRoute("{*allaspx}", new {allaspx = @".*\.aspx(/.*)?"});
            routes.IgnoreRoute("{*allaxd}", new { allaxd = @".*\.axd(/.*)?" });
            routes.IgnoreRoute("favicon.ico");
            routes.IgnoreRoute("favicon.png");

            RouteAttribute.MapDecoratedRoutes(routes);

            // MUST be the last route as a catch-all!
            //routes.MapRoute(name: "404-PageNotFound", url: "{*url}", defaults: new { controller = "Errors", action = "PageNotFound" });
        }

        private static void AddGlobalFilters()
        {
            GlobalFilters.Filters.Add(new ElmahHandleErrorAttribute());
            GlobalFilters.Filters.Add(new ReadOnlyModeErrorFilter());
            GlobalFilters.Filters.Add(new HandleErrorAttribute());
        }

        private static void AddModelBinders()
        {
            ModelBinders.Binders.Add(typeof(EditAssetRequest), new EditAssetModelBinder());
        }

        private static void NinjectPreStart()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            NinjectBootstrapper.Initialize(() => Container.Kernel);
        }

        private static void NinjectStop()
        {
            NinjectBootstrapper.ShutDown();
        }
    }
}