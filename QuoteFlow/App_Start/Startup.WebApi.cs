using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Routing;
using System.Web.Http.Tracing;
using Elmah.Contrib.WebApi;
using Newtonsoft.Json.Serialization;
using Ninject.Web.WebApi.OwinHost;
using Owin;
using QuoteFlow.Infrastructure;

namespace QuoteFlow
{
    public partial class Startup
    {
        public void ConfigureWebApi(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            // enable these to view information about the deserialization process and what not
//            SystemDiagnosticsTraceWriter traceWriter = config.EnableSystemDiagnosticsTracing();
//            traceWriter.IsVerbose = true;
//            traceWriter.MinimumLevel = TraceLevel.Debug;

            // Incorporate extra Get methods while still maintaining original WebAPI REST functionality
            //config.Routes.MapHttpRoute("ApiWithAction", "Api/{controller}/{id}/{action}", new {id = RouteParameter.Optional});
            config.Routes.MapHttpRoute(
                name: "ApiWithActionPost",
                routeTemplate: "Api/{controller}/{id}/{action}",
                defaults: new { },
                constraints: new { action = @"[A-Za-z]+", httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );

            config.Routes.MapHttpRoute("DefaultApiWithId", "Api/{controller}/{id}", new { id = RouteParameter.Optional }, new { id = @"\d+" });
            config.Routes.MapHttpRoute("DefaultApiWithAction", "Api/{controller}/{action}");
            config.Routes.MapHttpRoute("DefaultApiGet", "Api/{controller}", new { action = "Get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("DefaultApiPost", "Api/{controller}", new { action = "Post" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("ApiWithDoubleParamDelete", "Api/{controller}/{id}/{secondaryId}", new { action = "Delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });

            // only use json for webapi output
            var jsonFormatter = new JsonMediaTypeFormatter
            {
                SerializerSettings = {ContractResolver = new CamelCasePropertyNamesContractResolver()}
            };
            config.Services.Replace(typeof(IContentNegotiator), new JsonContentNegotiator(jsonFormatter));

            // enable elmah
            config.Services.Add(typeof(IExceptionLogger), new ElmahExceptionLogger());

            // internally calls app.UseWebApi
            app.UseNinjectWebApi(config);
        }
    }
}