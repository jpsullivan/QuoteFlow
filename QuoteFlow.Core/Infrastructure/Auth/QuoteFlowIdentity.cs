using System;
using System.Security.Principal;
using System.Web.Security;
using Jil;

namespace QuoteFlow.Core.Infrastructure.Auth
{
    [Serializable]
    public class QuoteFlowIdentity : IIdentity
    {
        private readonly FormsAuthenticationTicket _ticket;
        private readonly QuoteFlowUserIdentity _identity;

        public QuoteFlowIdentity() { }

        public QuoteFlowIdentity(FormsAuthenticationTicket ticket)
        {
            _ticket = ticket;
            // deserialize the userdata back into the intra user identity
            var options = new Options(dateFormat: DateTimeFormat.ISO8601);
            _identity = JSON.Deserialize<QuoteFlowUserIdentity>(ticket.UserData, options);
        }

        public string Username
        {
            get { return _identity.Username; }
        }

        public int UserId
        {
            get { return _identity.UserId; }
        }

        public bool IsAdministrator
        {
            get { return _identity.IsAdministrator; }
        }

        public string AuthenticationType
        {
            get { return "Custom"; }
        }

        public bool IsAuthenticated
        {
            get { return _ticket != null; }
        }

        public string Name
        {
            get { return _identity.Username; }
        }

        public string ClientName
        {
            get { return _identity.ClientName; }
        }

        public DateTime? LastLoggedIn
        {
            get { return _identity.LastLoggedIn; }
        }

        public string[] Roles
        {
            get { return _identity.Roles; }
        }
    }

    /// <summary>
    /// Extra user custom data
    /// </summary>
    public class QuoteFlowUserIdentity
    {
        public QuoteFlowUserIdentity() { }

        public int UserId { get; set; }

        public string Username { get; set; }

        public bool IsAdministrator { get; set; }

        public string ClientName { get; set; }

        public DateTime? LastLoggedIn { get; set; }

        public string[] Roles { get; set; }
    }
}