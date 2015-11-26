using System;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Api.Asset.Search.Searchers.Util
{
    public class UserFitsNavigatorHelper
    {
        private readonly IUserService _userService;

        public UserFitsNavigatorHelper(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// This method checks if the user exists and will fit in the navigator, 
        /// and returns the value that should be shown in the navigator. It first
        /// checks if the user exists under the given name as the (lowercased) username.
        /// 
        /// If that fails and the user full name search option is enabled, it checks
        /// if the user exists with the given name as the fullname or email, if it does,
        /// null is returned because that means that the query will not fit in the 
        /// simple navigator.
        /// 
        /// If the user was found by any means, then the passed-in name is returned.
        /// </summary>
        /// <param name="name">The username or fullname of the user to search for.</param>
        /// <returns>The username of the user if found by username, null if found by fullname or not found at all.</returns>
        public string CheckUser(string name)
        {
            var username = FindUserName(name);
            if (username != null)
            {
                return username;
            }
            if (UserExistsByFullNameOrEmail(name))
            {
                return null;
            }

            return name;
        }

        private string FindUserName(string name)
        {
            var user = _userService.GetUser(name, null);
            return user?.Username;
        }

        private bool UserExistsByFullNameOrEmail(string name)
        {
            var users = _userService.GetUsers(1); // todo organization fix
            foreach (var user in users)
            {
                var fullName = user.FullName;
                var email = user.EmailAddress;
                if (fullName != null && fullName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
                if (email != null && email.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}