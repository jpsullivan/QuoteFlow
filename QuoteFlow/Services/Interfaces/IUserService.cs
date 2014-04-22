using System.Collections.Generic;
using System.Threading.Tasks;
using QuoteFlow.Models;

namespace QuoteFlow.Services.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Fetches a user record that matches either the username.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        User GetUser(int userId);

        /// <summary>
        /// Fetches a user record that matches either the email address.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        User GetUser(string email);

        /// <summary>
        /// Fetches a user record that matches either the username or the email address.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        User GetUser(string username, string email);

        /// <summary>
        /// Fetches all users within the system.
        /// </summary>
        /// <returns></returns>
        IEnumerable<User> GetUsers();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        IEnumerable<User> FindAllByEmailAddress(string emailAddress);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unconfirmedEmailAddress"></param>
        /// <param name="optionalUsername"></param>
        /// <returns></returns>
        IEnumerable<User> FindByUnconfirmedEmailAddress(string unconfirmedEmailAddress, string optionalUsername);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="fullName"></param>
        /// <returns></returns>
        User CreateUser(string email, string password, string fullName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        User CreateUser(User user);

        /// <summary>
        /// Updates a user.
        /// </summary>
        /// <param name="user">The <see cref="User"/> to update.</param>
        void UpdateUser(User user);

        /// <summary>
        /// Check if a user belongs to an organization
        /// </summary>
        /// <param name="userId">The Id for the user being looked up.</param>
        /// <param name="organizationId">The Id for the organization being checked against.</param>
        /// <returns></returns>
        bool BelongsToOrganization(int userId, int organizationId);

        /// <summary>
        /// Check if a user belongs to an organization
        /// </summary>
        /// <param name="user">A User object to use for looking up its respective organizations.</param>
        /// <param name="organizationId">The Id for the organization being checked against.</param>
        /// <returns></returns>
        bool BelongsToOrganization(User user, int organizationId);

        /// <summary>
        /// Checks if a user is a member of more than one organization.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool BelongsToMultipleOrganizations(int userId);

        /// <summary>
        /// Checks if a user is a member of more than one organization.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        bool BelongsToMultipleOrganizations(User user);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="catalogId"></param>
        /// <returns></returns>
        bool IsCatalogCreator(User user, int catalogId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="catalog"></param>
        /// <returns></returns>
        bool IsCatalogCreator(User user, Catalog catalog);

        /// <summary>
        /// Check if a user is a catalog member based on whether the organizations said user
        /// is a member of coincide with the organizationId of the passed catalog.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="catalog"></param>
        /// <returns></returns>
        bool IsCatalogMember(User user, Catalog catalog);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="catalog"></param>
        /// <returns></returns>
        bool IsCatalogMember(int userId, Catalog catalog);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="catalogId"></param>
        /// <returns></returns>
        bool IsCatalogMember(int userId, int catalogId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="catalogId"></param>
        /// <returns></returns>
        bool IsCatalogAdmin(User user, int catalogId);

        /// <summary>
        /// Sets whether or not the specified user should be allowed
        /// to receive email.
        /// </summary>
        /// <param name="user">The <see cref="User"/> to modify.</param>
        /// <param name="emailAllowed">True if email should be allowed.</param>
        void ChangeEmailSubscription(User user, bool emailAllowed);

        /// <summary>
        /// Confirms the users' email address.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> ConfirmEmailAddress(User user, string token);

        /// <summary>
        /// Changes the users' email address.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="newEmailAddress"></param>
        /// <returns></returns>
        Task ChangeEmailAddress(User user, string newEmailAddress);

        /// <summary>
        /// Fetches any credentials for the specified <see cref="User"/>.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        ICollection<Credential> GetUserCredentials(int userId);

        /// <summary>
        /// Fetches the roles that the supplied <see cref="User"/> is assigned to.
        /// </summary>
        /// <param name="userId">The user id to use when searching for roles.</param>
        /// <returns></returns>
        ICollection<Role> GetUserRoles(int userId);
    }
}