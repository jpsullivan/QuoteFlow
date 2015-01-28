using Ninject.Web.Common.OwinHost;
using Owin;
using QuoteFlow.Core.DependencyResolution;

namespace QuoteFlow
{
    public partial class Startup
    {
        public void ConfigureNinject(IAppBuilder app)
        {
            app.UseNinjectMiddleware(() => Container.Kernel);
        }
    }
}