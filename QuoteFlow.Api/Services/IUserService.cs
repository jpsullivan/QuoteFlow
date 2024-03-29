﻿using System.Collections.Generic;
using System.Threading.Tasks;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Search;

namespace QuoteFlow.Api.Services
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
        /// Fetches all users that belong to a specific organization Id.
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        IEnumerable<User> GetUsers(int organizationId);

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
        /// Attempts to update the users username, validating it is a 
        /// valid one in the process.
        /// </summary>
        /// <param name="user"></param>
        void ChangeUsername(User user);

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

        /// <summary>
        /// Fetches all of the catalogs that a user has access to
        /// based on their organization access.
        /// </summary>
        /// <param name="userId">The id of the user to get catalogs for.</param>
        /// <returns></returns>
        IEnumerable<Catalog> GetCatalogs(int userId);

        /// <summary>
        /// Fetches all of the catalogs that a user has access to
        /// based on their organization access.
        /// </summary>
        /// <param name="user">The <see cref="User"/> to get catalogs for.</param>
        /// <returns></returns>
        IEnumerable<Catalog> GetCatalogs(User user);
            
        /// <summary>
        /// Fetches all of the organizations that a specific <see cref="User"/>
        /// is a member of.
        /// </summary>
        /// <param name="userId">The identifier of the user to lookup</param>
        /// <returns></returns>
        ICollection<Organization> GetUserOrganizations(int userId);

        /// <summary>
        /// Determines whether or not a user has access to view the
        /// asset details for a particular asset.
        /// </summary>
        /// <param name="user">The <see cref="User"/>.</param>
        /// <param name="asset">The <see cref="QuoteFlow.Core.Asset"/>.</param>
        /// <returns></returns>
        bool CanViewAsset(User user, Api.Models.Asset asset);

        /// <summary>
        /// Determine whether a user matches the search criteria specified in the 
        /// <param name="searchParams"></param>.
        /// </summary>
        /// <param name="user">The user to be matched</param>
        /// <param name="searchParams">The search criteria</param>
        /// <returns>True if the user matches the search criteria</returns>
        bool UserMatches(User user, UserSearchParams searchParams);
    }
}