using System;
using System.IO;
using System.Web;
using System.Web.Routing;
using Ninject;
using QuoteFlow.Core.DependencyResolution;
using QuoteFlow.Core.Services;

namespace QuoteFlow.Infrastructure.Handlers
{
    public class ManufacturerLogoRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var manufacturerLogoService = Container.Kernel.TryGet<ManufacturerLogoService>();

            int manufacturerId = Convert.ToInt32(requestContext.RouteData.Values["id"]);
            var manufacturerLogo = manufacturerLogoService.GetByManufacturerId(manufacturerId);

            if (manufacturerId == 0)
            {
                // return a 404 HttpHandler here
            }
            else
            {
                if (manufacturerLogo == null)
                {
                    return null;
                }

                requestContext.HttpContext.Response.Clear();
                requestContext.HttpContext.Response.ContentType = GetContentType(manufacturerLogo.Url);

                // find physical path to image here.  
                string filepath = requestContext.HttpContext.Server.MapPath(string.Format("~/App_Data/Files/manufacturer-logos/{0}", manufacturerLogo.Url));

                requestContext.HttpContext.Response.WriteFile(filepath);
                requestContext.HttpContext.Response.End();

            }

            return null;
        }

        private static string GetContentType(String path)
        {
            switch (Path.GetExtension(path))
            {
                case ".bmp": return "Image/bmp";
                case ".gif": return "Image/gif";
                case ".jpg": return "Image/jpeg";
                case ".png": return "Image/png";
            }
            return "";
        }
    }
}