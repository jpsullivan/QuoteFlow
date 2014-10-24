using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using QuoteFlow.Infrastructure.IoC;

namespace QuoteFlow
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            // Incorporate extra Get methods while still maintaining original WebAPI REST functionality
            config.Routes.MapHttpRoute("DefaultApiWithId", "Api/{controller}/{id}", new { id = RouteParameter.Optional }, new { id = @"\d+" });
            config.Routes.MapHttpRoute("DefaultApiWithAction", "Api/{controller}/{action}");
            config.Routes.MapHttpRoute("DefaultApiGet", "Api/{controller}", new { action = "Get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("DefaultApiPost", "Api/{controller}", new { action = "Post" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("ApiWithDoubleParamDelete", "Api/{controller}/{id}/{secondaryId}", new { action = "Delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });

            // Prevent XML from being the default return type, but keep it available for explicit use
            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

            // allow ninject to work as the DI container
            config.DependencyResolver = new NinjectWebApiDependencyResolver(Container.Kernel);

//            config.Services.Remove(typeof(ValueProviderFactory), config.Services.GetValueProviderFactories().First(f => f is QueryStringValueProviderFactory));
//            config.Services.Add(typeof(ValueProviderFactory), new QueryStringUniqueValueProviderFactory());
        }
    }
}
