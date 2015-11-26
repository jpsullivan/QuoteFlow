using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Asset.Comparator;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Search;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Core.Services
{
    public class UserSearcherHelperService : IUserSearcherHelperService
    {
        private readonly IUserService _userService;

        public UserSearcherHelperService(IUserService userService)
        {
            _userService = userService;
        }

        public void AddUserSuggestionParams(User user, ICollection<string> selectedUsers, IDictionary<string, object> templateParams)
        {
            var hasUserPickingPermission = HasUserPickingPermission(user);
            templateParams.Add("hasPermissionToPickUsers", hasUserPickingPermission);
            templateParams.Add("suggestedUsers", GetSuggestedUsers(user, selectedUsers, UserSearchParams.ActiveUsersAllowEmptyQuery));
            if (hasUserPickingPermission)
            {
                templateParams.Add("placholderText", "Find Users...");
            }
            else
            {
                templateParams.Add("placeholderText", "Enter username");
            }
        }

        public void AddGroupSuggestionParams(User user, IDictionary<string, object> templateParams)
        {
            throw new System.NotImplementedException();
        }

        public void AddUserGroupSuggestionParams(User user, ICollection<string> selectedUsers, IDictionary<string, object> templateParams)
        {
            throw new System.NotImplementedException();
        }

        public bool HasUserPickingPermission(User user)
        {
            return true;
        }

        public void AddUserGroupSuggestionParams(User user, IEnumerable<string> selectedUsers, UserSearchParams searchParams,
            IDictionary<string, object> templateParams)
        {
            throw new System.NotImplementedException();
        }

        private List<User> GetSuggestedUsers(User user, ICollection<string> selectedUsernames, UserSearchParams searchParams)
        {
            if (HasUserPickingPermission(user))
            {
                const int limit = 5;
                var suggestedUsers = new List<User>();
                
                // With a significant number of users in the system, a full sort is very slow
                // and pointless to suggest random users anyway. On the other hand, we do like
                // putting *something* in here for evaluators...
                if (suggestedUsers.Count < limit)
                {
                    var allUsers = _userService.GetUsers(1).ToList(); // todo organization fix
                    allUsers.Sort(new UserCachingComparer());
                    AddToSuggestList(limit, suggestedUsers, allUsers, selectedUsernames, searchParams);
                }

                // return a copy
                return new List<User>(suggestedUsers);
            }

            return null;
        }

        private void AddToSuggestList(int limit, ICollection<User> suggestedUsers, IEnumerable<User> users,
            ICollection<string> alreadySelected, UserSearchParams searchParams)
        {
            foreach (var user in users)
            {
                if (suggestedUsers.Count >= limit)
                {
                    break;
                }

                if (!suggestedUsers.Contains(user) && !alreadySelected.Contains(user.Username) &&
                    _userService.UserMatches(user, searchParams))
                {
                    suggestedUsers.Add(user);
                }
            }
        }
    }
}