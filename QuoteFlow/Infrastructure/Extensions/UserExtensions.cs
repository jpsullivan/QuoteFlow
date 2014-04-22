using System.Net.Mail;
using QuoteFlow.Models;

namespace QuoteFlow.Infrastructure.Extensions
{
    public static class UserExtensions
    {
        public static MailAddress ToMailAddress(this User user)
        {
            return new MailAddress(user.EmailAddress, user.Username);
        }
    }
}