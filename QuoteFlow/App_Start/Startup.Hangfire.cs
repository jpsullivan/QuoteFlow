using Hangfire;
using Hangfire.SqlServer;
using Owin;
using QuoteFlow.Core.DependencyResolution;

namespace QuoteFlow
{
    public partial class Startup
    {
        public void ConfigureHangfire(IAppBuilder app)
        {
            app.UseHangfire(config =>
            {
                config.UseSqlServerStorage("QuoteFlow.SqlServer");
                config.UseNinjectActivator(HangfireContainer.Kernel);
                config.UseServer();
            });
        }
    }
}