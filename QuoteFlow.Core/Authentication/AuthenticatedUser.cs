using System;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Authentication
{
    public class AuthenticatedUser
    {
        public User User { get; private set; }
        public Credential CredentialUsed { get; private set; }

        public AuthenticatedUser(User user, Credential cred)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (cred == null)
            {
                throw new ArgumentNullException(nameof(cred));
            }

            User = user;
            CredentialUsed = cred;
        }
    }
}
