using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Elmah;
using Elmah.Contrib.Mvc;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject.Web.Common;
using QuoteFlow;
using QuoteFlow.Infrastructure.Attributes;
using WebActivatorEx;
using RouteAttribute = QuoteFlow.Infrastructure.Attributes.RouteAttribute;

[assembly: System.Web.PreApplicationStartMethod(typeof(AppActivator), "PreStart")]
[assembly: PostApplicationStartMethod(typeof(AppActivator), "PostStart")]
[assembly: ApplicationShutdownMethod(typeof(AppActivator), "Stop")]

namespace QuoteFlow
{
    public static class AppActivator
    {
        private static readonly Bootstrapper NinjectBootstrapper = new Bootstrapper();

        public static void PreStart()
        {
            NinjectPreStart();
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
            DynamicModuleUtility.RegisterModule(typeof(Glimpse.AspNet.HttpModule));
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
        }

        /// <summary>
        /// Initializes and registers all of the routers that are used within
        /// the application. This includes Web Api, SignalR, and the standard MVC routes.
        /// </summary>
        private static void RegisterAllRouters()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

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
            routes.MapRoute("{*url}", new { controller = "Error", action = "PageNotFound" }.ToString());
        }

        private static void AddGlobalFilters()
        {
            GlobalFilters.Filters.Add(new ElmahHandleErrorAttribute());
            GlobalFilters.Filters.Add(new ReadOnlyModeErrorFilter());
            GlobalFilters.Filters.Add(new HandleErrorAttribute());
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