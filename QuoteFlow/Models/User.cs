using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Authentication;

namespace QuoteFlow.Models
{
    public class User
    {
        public User()
        {}

        public User(string username)
        {
            Credentials = new List<Credential>();
            Roles = new List<Role>();
            Username = username;
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public string UnconfirmedEmailAddress { get; set; }
        public string FullName { get; set; }
        public virtual ICollection<EmailMessage> Messages { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public bool EmailAllowed { get; set; }
        public string EmailConfirmationToken { get; set; }
        public string PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpirationDate { get; set; }
        public DateTime? CreatedUtc { get; set; }
        public DateTime? LastLoginUtc { get; set; }
        public virtual ICollection<Credential> Credentials { get; set; }
        public int[] Organizations { get; set; }
        public bool IsAdmin { get; set; }

        public bool Confirmed
        {
            get { return !String.IsNullOrEmpty(EmailAddress); }
        }

        public string LastSavedEmailAddress
        {
            get
            {
                return UnconfirmedEmailAddress ?? EmailAddress;
            }
        }

        public void ConfirmEmailAddress()
        {
            if (String.IsNullOrEmpty(UnconfirmedEmailAddress))
            {
                throw new InvalidOperationException("User does not have an email address to confirm");
            }
            EmailAddress = UnconfirmedEmailAddress;
            EmailConfirmationToken = null;
            UnconfirmedEmailAddress = null;
        }

        public void UpdateEmailAddress(string newEmailAddress, Func<string> generateToken)
        {
            if (!String.IsNullOrEmpty(UnconfirmedEmailAddress))
            {
                if (String.Equals(UnconfirmedEmailAddress, newEmailAddress, StringComparison.Ordinal))
                {
                    return; // already set as latest (unconfirmed) email address
                }
            }
            else
            {
                if (String.Equals(EmailAddress, newEmailAddress, StringComparison.Ordinal))
                {
                    return; // already set as latest (confirmed) email address
                }
            }

            UnconfirmedEmailAddress = newEmailAddress;
            EmailConfirmationToken = generateToken();
        }

        public bool HasPassword()
        {
            return Credentials.Any(c =>
                c.Type.StartsWith(CredentialTypes.Password.Prefix, StringComparison.OrdinalIgnoreCase));
        }
    }
}