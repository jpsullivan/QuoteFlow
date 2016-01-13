using System;
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
            GlobalConfiguration.Configuration.UseNinjectActivator(HangfireContainer.Kernel);

            // background indexing can take a while, 2 hours should be plenty of time
            var sqlServerOpts = new SqlServerStorageOptions {InvisibilityTimeout = TimeSpan.FromHours(2)};
            GlobalConfiguration.Configuration.UseSqlServerStorage("QuoteFlow.SqlServer", sqlServerOpts);

            //GlobalConfiguration.Configuration.UseElmahLogProvider();

            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
} 