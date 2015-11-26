using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using QuoteFlow.Api.Jql.Resolver;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Core.Jql.Resolver
{
    /// <summary>
    /// Resolves <see cref="User"/> objects and their names.
    /// </summary>
    public class UserResolver : INameResolver<User>
    {
        public IUserService UserService { get; protected set; }

        public UserResolver(IUserService userService)
        {
            UserService = userService;
        }

        public List<string> GetIdsFromName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var user = UserService.GetUser(name, null);
            if (user != null)
            {
                return new List<string>(user.Id);
            }

            return GetUsersFromFullNameOrEmail(name);
        }

        public bool NameExists(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (UserService.GetUser(name, null) != null)
            {
                return true;
            }

            return HasAnyFullNameOrEmailMatches(name);
        }

        public bool IdExists(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            return NameExists(id.ToString());
        }

        /// <summary>
        /// Picks between the matched full name and emails matches. If both full name
        /// and email matches are available, then the email matches are used if the name 
        /// looks like an email address; otherwise, the full name matches are used.
        /// </summary>
        /// <param name="name">The name to find matches for</param>
        /// <param name="fullNameMatches">The names of users whose full names matched</param>
        /// <param name="emailMatches">The names of users who email addresses matched</param>
        /// <returns>A list of user IDs for the users that best match the supplied <param name="name"></param></returns>
        private List<string> PickEmailOrFullNameMatches(string name, List<string> fullNameMatches, List<string> emailMatches)
        {
            if (!emailMatches.Any())
            {
                return fullNameMatches;
            }

            if (!fullNameMatches.Any() || IsEmail(name))
            {
                return emailMatches;
            }

            return fullNameMatches;
        }
        
        private List<string> GetUsersFromFullNameOrEmail(string name)
        {
            var fullNameMatches = new List<string>();
            var emailMatches = new List<string>();
            foreach (var user in UserService.GetUsers(1)) // todo organization fix
            {
                if (name.Equals(user.FullName, StringComparison.InvariantCultureIgnoreCase))
                {
                    fullNameMatches.Add(user.Id.ToString());
                }

                if (name.Equals(user.EmailAddress, StringComparison.InvariantCultureIgnoreCase))
                {
                    emailMatches.Add(user.Id.ToString());
                }
            }

            return PickEmailOrFullNameMatches(name, fullNameMatches, emailMatches);
        }

        private bool HasAnyFullNameOrEmailMatches(string name)
        {
            foreach (var user in UserService.GetUsers())
            {
                return name.Equals(user.FullName, StringComparison.InvariantCultureIgnoreCase) ||
                       name.Equals(user.EmailAddress, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }

        /// <summary>
        /// Eww. http://stackoverflow.com/a/16168103/1487071
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool IsEmail(string name)
        {
            return Regex.IsMatch(name,
                @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                RegexOptions.IgnoreCase);
        }

        public User Get(int id)
        {
            return UserService.GetUser(id);
        }

        public ICollection<User> GetAll()
        {
            return UserService.GetUsers(1).ToList(); // todo oranization fix
        }
    }
}