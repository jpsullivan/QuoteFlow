using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Ninject;
using Owin;
using QuoteFlow.Core.Authentication;
using QuoteFlow.Core.Configuration;
using QuoteFlow.Core.DependencyResolution;

[assembly: OwinStartup(typeof(QuoteFlow.Startup))]
namespace QuoteFlow
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureNinject(app);

            // Get config
            var config = Container.Kernel.Get<ConfigurationService>();
            var auth = Container.Kernel.Get<AuthenticationService>();

            // Configure logging
            app.SetLoggerFactory(new DiagnosticsLoggerFactory());

            if (config.Current.RequireSSL)
            {
                // Put a middleware at the top of the stack to force the user over to SSL
                // if authenticated.
                app.UseForceSslWhenAuthenticated(config.Current.SSLPort);
            }

            ConfigureAuth(config, auth, app);
            ConfigureWebApi(app);
            ConfigureHangfire(app);
        }
    }
}