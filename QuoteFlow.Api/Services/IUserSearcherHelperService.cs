using System.Collections.Generic;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Search;

namespace QuoteFlow.Api.Services
{
    public interface IUserSearcherHelperService
    {
        void AddUserSuggestionParams(User user, ICollection<string> selectedUsers, IDictionary<string, object> templateParams);

        void AddGroupSuggestionParams(User user, IDictionary<string, object> templateParams);

        void AddUserGroupSuggestionParams(User user, ICollection<string> selectedUsers, IDictionary<string, object> templateParams);

        /// <summary>
        /// add user and group suggestions based on search parameters.
        /// the parameters will be added into the {@code params} parameter in-place.
        /// </summary>
        /// <param name="user"> the user requesting for the suggestions </param>
        /// <param name="selectedUsers"> a list of recently selected users, which could be included into the suggested users with higher priority </param>
        /// <param name="searchParams"> additional search parameters for groups and roles based restrictions. </param>
        /// <param name="templateParams"> the map to hold the parameters </param>
        void AddUserGroupSuggestionParams(User user, IEnumerable<string> selectedUsers, UserSearchParams searchParams, IDictionary<string, object> templateParams);

        /// <summary>
        /// Determine whether a user has permission to pick users.
        /// </summary>
        bool HasUserPickingPermission(User user);
    }
}