using System.Collections.Generic;

namespace QuoteFlow.Api.Services
{
    public interface IFormsAuthenticationService
    {
        void SetAuthCookie(
            string userName,
            bool createPersistentCookie,
            IEnumerable<string> roles);

        void SignOut();

        bool ShouldForceSSL(HttpContextBase context);
    }
}