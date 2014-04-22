using Ninject.Modules;
using QuoteFlow.Authentication.Providers;

namespace QuoteFlow.Authentication
{
    public class AuthNinjectModule : NinjectModule
    {
        public override void Load()
        {
            foreach (var instance in Authenticator.GetAllAvailable())
            {
                Bind(typeof(Authenticator))
                    .ToConstant(instance)
                    .InSingletonScope();
            }
        }
    }
}