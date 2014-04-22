using System.Web;
using Glimpse.Core.Extensibility;
using QuoteFlow.Configuration;

namespace QuoteFlow.Diagnostics
{
    public class GlimpseRuntimePolicy : IRuntimePolicy
    {
        public IAppConfiguration Configuration { get; protected set; }

        protected GlimpseRuntimePolicy()
        {
        }

        public GlimpseRuntimePolicy(IAppConfiguration configuration)
        {
            Configuration = configuration;
        }

        public RuntimeEvent ExecuteOn
        {
            get { return RuntimeEvent.BeginSessionAccess; }
        }

        public RuntimePolicy Execute(IRuntimePolicyContext policyContext)
        {
            return Execute(policyContext.GetRequestContext<HttpContextBase>());
        }

        public RuntimePolicy Execute(HttpContextBase context)
        {
            // Policy is: Localhost collects data, and local users who have logged in, have set the web config to enable diagnostics, and are SSL'd (if necessary) can see everything.
            if (context.Request.IsLocal && Configuration.DiagnosticsEnabled && context.Request.IsAuthenticated &&
                 (!Configuration.RequireSSL || context.Request.IsSecureConnection))
            {
                return RuntimePolicy.On;
            }
            if (context.Request.IsLocal)
            {
                return RuntimePolicy.PersistResults;
            }
            return RuntimePolicy.Off;
        }
    }
}