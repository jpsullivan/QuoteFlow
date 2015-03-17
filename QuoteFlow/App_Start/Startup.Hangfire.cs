using Hangfire;
using Hangfire.SqlServer;
using Owin;

namespace QuoteFlow
{
    public partial class Startup
    {
        public void ConfigureHangfire(IAppBuilder app)
        {
            app.UseHangfire(config =>
            {
                config.UseSqlServerStorage("QuoteFlow.SqlServer");
                config.UseServer();
            });
        }
    }
}