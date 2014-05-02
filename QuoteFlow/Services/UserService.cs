using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using QuoteFlow.Auditing;
using QuoteFlow.Configuration;
using QuoteFlow.Infrastructure.Exceptions;
using QuoteFlow.Infrastructure.Extensions;
using QuoteFlow.Models;
using QuoteFlow.Services.Interfaces;
using StackExchange.Profiling.Helpers.Dapper;
using Crypto = QuoteFlow.Services.CryptographyService;

namespace QuoteFlow.Services
{
    public class UserService : IUserService
    {
        #region IoC

        public IAppConfiguration Config { get; protected set; }
        public ICatalogService CatalogService { get; protected set; }
        public AuditingService Auditing { get; protected set; }

        protected UserService() { }

        public UserService(
            IAppConfiguration config,
            ICatalogService catalogService,
            AuditingService auditing)
            : this()
        {
            Config = config;
            CatalogService = catalogService;
            Auditing = auditing;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public User GetUser(int userId)
        {
            const string sql = "select * from Users where Id = @userId";
            var user = Current.DB.Query<User>(sql, new {userId}).FirstOrDefault();

            if (user == null) {
                return null;
            }

            user.Credentials = GetUserCredentials(userId);
            user.Roles = GetUserRoles(userId);
            user.Organizations = GetUserOrganizations(userId);

            return user;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public User GetUser(string email)
        {
            const string sql = "select * from Users where EmailAddress = @email";

            var user = Current.DB.Query<User>(sql, new { email }).FirstOrDefault();
            if (user == null) 
            {
                // todo: throw error if no user found
                return null; 
            }

            user.Credentials = GetUserCredentials(user.Id);
            user.Roles = GetUserRoles(user.Id);
            user.Organizations = GetUserOrganizations(user.Id);
            
            return user;
        }

        /// <summary>
        /// Fetches a user record that matches either the username or the email address.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public User GetUser(string username, string email)
        {
            const string sql = "select * from Users where Username = @username or EmailAddress = @email";

            var user = Current.DB.Query<User>(sql, new {username, email}).FirstOrDefault();
            if (user == null)
            {
                // todo: throw error if no user found
                return null;
            }

            user.Credentials = GetUserCredentials(user.Id);
            user.Roles = GetUserRoles(user.Id);
            user.Organizations = GetUserOrganizations(user.Id);

            return user;
        }

        /// <summary>
        /// Fetches all users within the system.
        /// </summary>
        /// <returns>A collection of all <see cref="User"/>s</returns>
        public IEnumerable<User> GetUsers()
        {
            var users = Current.DB.Users.All();

            var allUsers = users as IList<User> ?? users.ToList();
            foreach (var user in allUsers)
            {
                user.Credentials = GetUserCredentials(user.Id);
                user.Roles = GetUserRoles(user.Id);
                user.Organizations = GetUserOrganizations(user.Id);
            }

            return allUsers;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public IEnumerable<User> FindAllByEmailAddress(string emailAddress)
        {
            const string sql = "select * from Users where EmailAddress = @emailAddress";
            var users = Current.DB.Query<User>(sql, new {emailAddress});

            var findAllByEmailAddress = users as IList<User> ?? users.ToList();
            foreach (var user in findAllByEmailAddress) 
            {
                user.Credentials = GetUserCredentials(user.Id);
                user.Roles = GetUserRoles(user.Id);
                user.Organizations = GetUserOrganizations(user.Id);
            }

            return findAllByEmailAddress;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unconfirmedEmailAddress"></param>
        /// <param name="optionalUsername"></param>
        /// <returns></returns>
        public IEnumerable<User> FindByUnconfirmedEmailAddress(string unconfirmedEmailAddress, string optionalUsername)
        {
            string sql;
            
            if (optionalUsername == null) {
                sql = "select * from Users where UnconfirmedEmailAddress = @unconfirmedEmailAddress";
                return Current.DB.Query<User>(sql, new {unconfirmedEmailAddress});
            }

            sql = "select * from Users where UnconfirmedEmailAddress = @unconfirmedEmailAddress and Username = @optionalUsername";
            return Current.DB.Query<User>(sql, new { unconfirmedEmailAddress, optionalUsername });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public User CreateUser(string email, string password, string fullName)
        {
            var user = new User
            {
                EmailAddress = email,
                FullName = fullName,
                CreatedUtc = DateTime.UtcNow
            };

            var insertSuccess = Current.DB.Users.Insert(new
            {
                user.EmailAddress,
                user.FullName,
                user.CreatedUtc
            });

            if (insertSuccess != null)
            {
                user.Id = insertSuccess.Value;
            }

            return user;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public User CreateUser(User user)
        {
            if (user == null) 
                throw new ArgumentException("User cannot be null");

            var success = Current.DB.Users.Insert(new
            {
                user.Username,
                user.EmailAddress,
                user.UnconfirmedEmailAddress,
                user.FullName,
                user.IsAdmin,
                user.EmailAllowed,
                user.EmailConfirmationToken,
                user.PasswordResetToken,
                user.PasswordResetTokenExpirationDate,
                user.CreatedUtc,
                user.LastLoginUtc
            });

            if (success == null) {
                // insert failed. todo: throw message here
                return user;
            } 

            user.Id = success.Value;

            // now add in the credentials
            foreach (var cred in user.Credentials)
            {
                Current.DB.Credentials.Insert(new
                    {
                        UserId = user.Id,
                        Type = cred.Type,
                        Value = cred.Value,
                        Ident = cred.Ident
                    });
            }

            return user;
        }

        /// <summary>
        /// Updates a user.
        /// </summary>
        /// <param name="user">The <see cref="User"/> to update.</param>
        public void UpdateUser(User user)
        {
            var original = Current.DB.Users.Get(user.Id);

            // track which fields change on the object
            var s = Snapshotter.Start(original);
            original.Username = user.Username;
            original.EmailAddress = user.EmailAddress;
            original.UnconfirmedEmailAddress = user.UnconfirmedEmailAddress;
            original.FullName = user.FullName;
            original.Messages = user.Messages;
            original.Roles = user.Roles;
            original.EmailAllowed = user.EmailAllowed;
            original.EmailConfirmationToken = user.EmailConfirmationToken;
            original.PasswordResetToken = user.PasswordResetToken;
            original.PasswordResetTokenExpirationDate = user.PasswordResetTokenExpirationDate;
            original.Credentials = user.Credentials;
            original.Organizations = user.Organizations;
            original.IsAdmin = user.IsAdmin;

            var diff = s.Diff();
            if (diff.ParameterNames.Any())
            {
                Current.DB.Users.Update(user.Id, diff);
            }
        }

        /// <summary>
        /// Check if a user belongs to an organization
        /// </summary>
        /// <param name="userId">The Id for the user being looked up.</param>
        /// <param name="organizationId">The Id for the organization being checked against.</param>
        /// <returns></returns>
        public bool BelongsToOrganization(int userId, int organizationId)
        {
            var user = GetUser(userId);
            return user.Organizations.Any(org => org.Id == organizationId);
        }

        /// <summary>
        /// Check if a user belongs to an organization
        /// </summary>
        /// <param name="user">A User object to use for looking up its respective organizations.</param>
        /// <param name="organizationId">The Id for the organization being checked against.</param>
        /// <returns></returns>
        public bool BelongsToOrganization(User user, int organizationId)
        {
            return user != null && user.Organizations.Any(org => org.Id == organizationId);
        }

        /// <summary>
        /// Checks if a user is a member of more than one organization.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool BelongsToMultipleOrganizations(int userId)
        {
            var user = GetUser(userId);
            return BelongsToMultipleOrganizations(user);
        }

        /// <summary>
        /// Checks if a user is a member of more than one organization.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool BelongsToMultipleOrganizations(User user)
        {
            var orgs = user.Organizations;
            return orgs.Count() > 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="catalogId"></param>
        /// <returns></returns>
        public bool IsCatalogCreator(User user, int catalogId)
        {
            var catalog = CatalogService.GetCatalog(catalogId);
            return user.Id == catalog.CreatorId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="catalog"></param>
        /// <returns></returns>
        public bool IsCatalogCreator(User user, Catalog catalog)
        {
            return user.Id == catalog.CreatorId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="catalog"></param>
        /// <returns></returns>
        public bool IsCatalogMember(User user, Catalog catalog)
        {
            return user.Credentials.Any(org => org.Id == catalog.OrganizationId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="catalog"></param>
        /// <returns></returns>
        public bool IsCatalogMember(int userId, Catalog catalog)
        {
            var user = GetUser(userId);
            return IsCatalogMember(user, catalog);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="catalogId"></param>
        /// <returns></returns>
        public bool IsCatalogMember(int userId, int catalogId)
        {
            var user = GetUser(userId);
            var catalog = CatalogService.GetCatalog(catalogId);
            return IsCatalogMember(user, catalog);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="catalogId"></param>
        /// <returns></returns>
        public bool IsCatalogAdmin(User user, int catalogId)
        {
            var catalog = CatalogService.GetCatalog(catalogId);

            // Check if the user is either the creator or a member of the catalog admin list
            if (IsCatalogCreator(user, catalog))
            {
                // TODO: Check if the user is also an admin member of the catalog
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets whether or not the specified user should be allowed
        /// to receive email.
        /// </summary>
        /// <param name="user">The <see cref="User"/> to modify.</param>
        /// <param name="emailAllowed">True if email should be allowed.</param>
        public void ChangeEmailSubscription(User user, bool emailAllowed)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.EmailAllowed = emailAllowed;
            UpdateUser(user);
        }

        /// <summary>
        /// Confirms the users' email address.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="newEmailAddress"></param>
        /// <returns></returns>
        public async Task ChangeEmailAddress(User user, string newEmailAddress)
        {
            var existingUsers = FindAllByEmailAddress(newEmailAddress);
            if (existingUsers.AnySafe(u => u.Id != user.Id))
            {
                throw new EntityException(Strings.EmailAddressBeingUsed, newEmailAddress);
            }

            await Auditing.SaveAuditRecord(new UserAuditRecord(user, UserAuditAction.ChangeEmail, newEmailAddress));

            user.UpdateEmailAddress(newEmailAddress, Crypto.GenerateToken);
            UpdateUser(user);
        }

        /// <summary>
        /// Fetches any credentials for the specified <see cref="User"/>.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ICollection<Credential> GetUserCredentials(int userId)
        {
            const string sql = "select * from Credentials where UserId = @userId";
            return Current.DB.Query<Credential>(sql, new { userId }).ToList();
        }

        /// <summary>
        /// Fetches the roles that the supplied <see cref="User"/> is assigned to.
        /// </summary>
        /// <param name="userId">The user id to use when searching for roles.</param>
        /// <returns></returns>
        public ICollection<Role> GetUserRoles(int userId)
        {
            var builder = new SqlBuilder();
            SqlBuilder.Template tmpl = builder.AddTemplate(@"
                SELECT r.Id, r.Name 
                FROM UserRoles ur 
                /**join**/
                /**where**/"
                );

            builder.Join("Roles AS r ON r.Id = ur.RoleId");
            builder.Where("ur.UserId = @userId");

            return Current.DB.Query<Role>(tmpl.RawSql, new {userId}).ToList();
        }

        /// <summary>
        /// Fetches all of the organizations that a specific <see cref="User"/>
        /// is a member of.
        /// </summary>
        /// <param name="userId">The identifier of the user to lookup</param>
        /// <returns></returns>
        public ICollection<Organization> GetUserOrganizations(int userId)
        {
            var builder = new SqlBuilder();
            SqlBuilder.Template tmpl = builder.AddTemplate(@"
                SELECT o.Id, o.OrganizationName, o.OwnerId, o.CreatorId
                FROM OrganizationUsers ou 
                /**join**/
                /**where**/"
                );

            builder.Join("Organizations AS o ON o.Id = ou.OrganizationId");
            builder.Where("ou.UserId = @userId");

            return Current.DB.Query<Organization>(tmpl.RawSql, new { userId }).ToList();
        }

        /// <summary>
        /// Fetches all of the catalogs that a user has access to
        /// based on their organization access.
        /// </summary>
        /// <param name="userId">The id of the user to get catalogs for.</param>
        /// <returns></returns>
        public IEnumerable<Catalog> GetCatalogs(int userId)
        {
            if (userId == 0)
            {
                throw new ArgumentException("User ID must be greater than zero.");
            }

            var user = GetUser(userId);
            return CatalogService.GetCatalogsWithinOrganizations(user.Organizations);
        }

        /// <summary>
        /// Fetches all of the catalogs that a user has access to
        /// based on their organization access.
        /// </summary>
        /// <param name="user">The <see cref="User"/> to get catalogs for.</param>
        /// <returns></returns>
        public IEnumerable<Catalog> GetCatalogs(User user)
        {
            if (user == null) {
                throw new ArgumentException("User cannot be null");
            }

            return CatalogService.GetCatalogsWithinOrganizations(user.Organizations);
        }

        /// <summary>
        /// Confirms the users' email address.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<bool> ConfirmEmailAddress(User user, string token)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (String.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException("token");
            }

            if (user.EmailConfirmationToken != token)
            {
                return false;
            }

            var conflictingUsers = FindAllByEmailAddress(user.UnconfirmedEmailAddress);
            if (conflictingUsers.AnySafe(u => u.Id != user.Id))
            {
                throw new EntityException(Strings.EmailAddressBeingUsed, user.UnconfirmedEmailAddress);
            }

            await Auditing.SaveAuditRecord(new UserAuditRecord(user, UserAuditAction.ConfirmEmail, user.UnconfirmedEmailAddress));

            user.ConfirmEmailAddress();

            UpdateUser(user);
            return true;
        }
    }
}