using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin;
using QuoteFlow.Models;
using QuoteFlow.Services;

namespace QuoteFlow.Infrastructure.Extensions
{
    public static class AuthExtensions
    {
        public static string GetClaimOrDefault(this ClaimsPrincipal self, string claimType)
        {
            return self.Claims.GetClaimOrDefault(claimType);
        }

        public static string GetClaimOrDefault(this ClaimsIdentity self, string claimType)
        {
            return self.Claims.GetClaimOrDefault(claimType);
        }

        public static string GetClaimOrDefault(this IEnumerable<Claim> self, string claimType)
        {
            return self
                .Where(c => String.Equals(c.Type, claimType, StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Value)
                .FirstOrDefault();
        }

        // This is a method because the first call will perform a database call
        /// <summary>
        /// Get the current user, from the database, or if someone in this request has already
        /// retrieved it, from memory. This will NEVER return null. It will throw an exception
        /// that will yield an HTTP 401 if it would return null. As a result, it should only
        /// be called in actions with the Authorize attribute or a Request.IsAuthenticated check
        /// </summary>
        /// <returns>The current user</returns>
        public static User GetCurrentUser(this IOwinContext self)
        {
            if (self.Request.User == null)
            {
                return null;
            }

            User user = null;
            object obj;
            if (self.Environment.TryGetValue(Constants.CurrentUserOwinEnvironmentKey, out obj))
            {
                user = obj as User;
            }

            if (user == null)
            {
                user = LoadUser(self);
                self.Environment[Constants.CurrentUserOwinEnvironmentKey] = user;
            }

            if (user == null)
            {
                // Unauthorized! If we get here it's because a valid session token was presented, but the
                // user doesn't exist any more. So we just have a generic error.
                throw new HttpException(401, Strings.Unauthorized);
            }

            return user;
        }

        private static User LoadUser(IOwinContext context)
        {
            var principal = context.Authentication.User;
            
            if (principal == null) return null; // No user logged in, or credentials could not be resolved
            
            // Try to authenticate with the user name
            string userName = principal.GetClaimOrDefault(ClaimTypes.Name);

            if (userName.HasValue())
            {
                return DependencyResolver
                    .Current
                    .GetService<UserService>()
                    .GetUser(userName, null); // force a username-only lookup
            }
            return null; // No user logged in, or credentials could not be resolved
        }
    }
}