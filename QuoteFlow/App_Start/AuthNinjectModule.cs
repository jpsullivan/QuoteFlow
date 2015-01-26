using Ninject.Modules;
using QuoteFlow.Core.Authentication.Providers;

namespace QuoteFlow
{
    public class AuthNinjectModule : NinjectModule
    {
        public override void Load()
        {
            foreach (var instance in Authenticator.GetAllAvailable())
            {
                Bind(typeof(Authenticator)).ToConstant(instance).InSingletonScope();
            }
        }
    }
}