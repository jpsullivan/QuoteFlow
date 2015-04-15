using Hangfire;
using Owin;
using QuoteFlow.Core.DependencyResolution;

namespace QuoteFlow
{
    public partial class Startup
    {
        public void ConfigureHangfire(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseNinjectActivator(HangfireContainer.Kernel);
            GlobalConfiguration.Configuration.UseSqlServerStorage("QuoteFlow.SqlServer");
            GlobalConfiguration.Configuration.UseElmahLogProvider();

            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
} 