using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Owin;
using QuoteFlow.Api.Authentication;
using QuoteFlow.Api.Configuration;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Models.ViewModels;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Auditing;
using QuoteFlow.Core.Authentication.Providers;
using QuoteFlow.Core.Diagnostics;
using QuoteFlow.Core.Infrastructure.Auth;
using QuoteFlow.Core.Infrastructure.Exceptions;
using QuoteFlow.Core.Services;

namespace QuoteFlow.Core.Authentication
{
    public class AuthenticationService
    {
        public IAppConfiguration Config { get; private set; }
        public IDictionary<string, Authenticator> Authenticators { get; private set; }
        public IUserService UserService { get; private set; }
        public AuditingService Auditing { get; private set; }

        private IDiagnosticsSource Trace { get; set; }

        private readonly Dictionary<string, Func<string, string>> _credentialFormatters;

        protected AuthenticationService()
            : this(null, null, null, AuditingService.None, Enumerable.Empty<Authenticator>())
        {
        }

        public AuthenticationService(IAppConfiguration config, IDiagnosticsService diagnostics, IUserService userService, AuditingService auditing, IEnumerable<Authenticator> providers)
        {
            _credentialFormatters = new Dictionary<string, Func<string, string>>(StringComparer.OrdinalIgnoreCase) {
                { "password", _ => Strings.CredentialType_Password },
                { "apikey", _ => Strings.CredentialType_ApiKey },
                { "external", FormatExternalCredentialType }
            };

            Config = config;
            Auditing = auditing;
            Trace = diagnostics.SafeGetSource("AuthenticationService");
            Authenticators = providers.ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
            UserService = userService;
        }

        public virtual async Task<AuthenticatedUser> Authenticate(string userNameOrEmail, string password)
        {
            using (Trace.Activity("Authenticate:" + userNameOrEmail))
            {
                var user = FindByUserNameOrEmail(userNameOrEmail);

                // Check if the user exists
                if (user == null)
                {
                    Trace.Information("No such user: " + userNameOrEmail);
                    return null;
                }

                // Validate the password
                Credential matched;
                if (!ValidatePasswordCredential(user.Credentials, password, out matched))
                {
                    Trace.Information("Password validation failed: " + userNameOrEmail);
                    return null;
                }

                var passwordCredentials = user
                    .Credentials
                    .Where(c => c.Type.StartsWith(CredentialTypes.Password.Prefix, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                if (passwordCredentials.Count > 1 || !passwordCredentials.Any(c => String.Equals(c.Type, CredentialTypes.Password.Pbkdf2, StringComparison.OrdinalIgnoreCase)))
                {
                    await MigrateCredentials(user, passwordCredentials, password);
                }

                // Return the result
                Trace.Verbose("Successfully authenticated '" + user.Username + "' with '" + matched.Type + "' credential");
                return new AuthenticatedUser(user, matched);
            }
        }

        public virtual AuthenticatedUser Authenticate(Credential credential)
        {
            if (credential.Type.StartsWith(CredentialTypes.Password.Prefix, StringComparison.OrdinalIgnoreCase))
            {
                // Password credentials cannot be used this way.
                throw new ArgumentException(Strings.PasswordCredentialsCannotBeUsedHere, "credential");
            }

            using (Trace.Activity("Authenticate Credential: " + credential.Type))
            {
                var matched = FindMatchingCredential(credential);

                if (matched == null)
                {
                    Trace.Information("No user matches credential of type: " + credential.Type);
                    return null;
                }

                Trace.Verbose("Successfully authenticated '" + matched.User.Username + "' with '" + matched.Type + "' credential");
                return new AuthenticatedUser(matched.User, matched);
            }
        }

        public virtual void CreateSession(IOwinContext owinContext, User user)
        {
            // Create a claims identity for the session
            ClaimsIdentity identity = CreateIdentity(user, AuthenticationTypes.LocalUser);

            // Issue the session token and clean up the external token if present
            owinContext.Authentication.SignIn(identity);
            owinContext.Authentication.SignOut(AuthenticationTypes.External);
        }

        public virtual async Task<AuthenticatedUser> Register(string username, string emailAddress, string fullname, Credential credential)
        {
            var existingUser = UserService.GetUser(username, emailAddress);
            if (existingUser != null) {
                if (String.Equals(existingUser.Username, username, StringComparison.OrdinalIgnoreCase))
                {
                    throw new EntityException(Strings.UsernameNotAvailable, username);
                }
                throw new EntityException(Strings.EmailAddressBeingUsed, emailAddress);
            }

            var newUser = new User(username)
            {
                FullName = fullname,
                EmailAllowed = true,
                UnconfirmedEmailAddress = emailAddress,
                EmailConfirmationToken = CryptographyService.GenerateToken(),
                CreatedUtc = DateTime.UtcNow
            };

            // Add a credential for the password
            newUser.Credentials.Add(credential);

            if (!Config.ConfirmEmailAddresses)
            {
                newUser.ConfirmEmailAddress();
            }

            // Write an audit record
            await Auditing.SaveAuditRecord(new UserAuditRecord(newUser, UserAuditAction.Registered));

            UserService.CreateUser(newUser);

            return new AuthenticatedUser(newUser, credential);
        }

        public virtual Task ReplaceCredential(string username, Credential credential)
        {
            var user = UserService.GetUser(username);
            if (user == null)
            {
                throw new InvalidOperationException(Strings.UserNotFound);
            }
            return ReplaceCredential(user, credential);
        }

        public virtual async Task ReplaceCredential(User user, Credential credential)
        {
            await ReplaceCredentialInternal(user, credential);
        }

        public virtual async Task<Credential> ResetPasswordWithToken(string username, string token, string newPassword)
        {
            if (String.IsNullOrEmpty(newPassword))
            {
                throw new ArgumentNullException("newPassword");
            }

            var user = UserService.GetUser(username);
            
            if (user == null || !String.Equals(user.PasswordResetToken, token, StringComparison.Ordinal) ||
                user.PasswordResetTokenExpirationDate.IsInThePast()) return null;

            if (!user.Confirmed)
            {
                throw new InvalidOperationException(Strings.UserIsNotYetConfirmed);
            }

            var cred = CredentialBuilder.CreatePbkdf2Password(newPassword);
            await ReplaceCredentialInternal(user, cred);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpirationDate = null;
            UserService.UpdateUser(user);
            return cred;
        }

        public virtual async Task<User> GeneratePasswordResetToken(string usernameOrEmail, int expirationInMinutes)
        {
            if (String.IsNullOrEmpty(usernameOrEmail))
            {
                throw new ArgumentNullException("usernameOrEmail");
            }
            if (expirationInMinutes < 1)
            {
                throw new ArgumentException(
                    "Token expiration should give the user at least a minute to change their password", "expirationInMinutes");
            }
            var user = FindByUserNameOrEmail(usernameOrEmail);
            if (user == null)
            {
                return null;
            }
            await GeneratePasswordResetToken(user, expirationInMinutes);
            return user;
        }

        public virtual async Task GeneratePasswordResetToken(User user, int expirationInMinutes)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (expirationInMinutes < 1)
            {
                throw new ArgumentException(
                    "Token expiration should give the user at least a minute to change their password", "expirationInMinutes");
            }

            if (!user.Confirmed)
            {
                throw new InvalidOperationException(Strings.UserIsNotYetConfirmed);
            }

            if (user.PasswordResetToken.HasValue() && !user.PasswordResetTokenExpirationDate.IsInThePast())
            {
                return;
            }

            user.PasswordResetToken = CryptographyService.GenerateToken();
            user.PasswordResetTokenExpirationDate = DateTime.UtcNow.AddMinutes(expirationInMinutes);

            await Auditing.SaveAuditRecord(new UserAuditRecord(user, UserAuditAction.RequestedPasswordReset));

            UserService.UpdateUser(user);
        }

        public virtual async Task<bool> ChangePassword(User user, string oldPassword, string newPassword)
        {
            var hasPassword = user.Credentials.Any(
                c => c.Type.StartsWith(CredentialTypes.Password.Prefix, StringComparison.OrdinalIgnoreCase));
            Credential _;

            if (hasPassword && !ValidatePasswordCredential(user.Credentials, oldPassword, out _))
            {
                // Invalid old password!
                return false;
            }

            // Replace/Set password credential
            var cred = CredentialBuilder.CreatePbkdf2Password(newPassword);
            await ReplaceCredentialInternal(user, cred);
            return true;
        }

        public virtual ActionResult Challenge(string providerName, string redirectUrl)
        {
            Authenticator provider;
            if (!Authenticators.TryGetValue(providerName, out provider))
            {
                throw new InvalidOperationException(String.Format(
                    CultureInfo.CurrentCulture,
                    Strings.UnknownAuthenticationProvider,
                    providerName));
            }
            if (!provider.BaseConfig.Enabled)
            {
                throw new InvalidOperationException(String.Format(
                    CultureInfo.CurrentCulture,
                    Strings.AuthenticationProviderDisabled,
                    providerName));
            }

            return provider.Challenge(redirectUrl);
        }

        public virtual async Task AddCredential(User user, Credential credential)
        {
            await Auditing.SaveAuditRecord(new UserAuditRecord(user, UserAuditAction.AddedCredential, credential));
            Current.DB.Credentials.Insert(credential);
        }

        public virtual CredentialViewModel DescribeCredential(Credential credential)
        {
            var kind = GetCredentialKind(credential.Type);
            Authenticator auther = null;
            if (kind == CredentialKind.External)
            {
                string providerName = credential.Type.Split('.')[1];
                if (!Authenticators.TryGetValue(providerName, out auther))
                {
                    auther = null;
                }
            }

            return new CredentialViewModel()
            {
                Type = credential.Type,
                TypeCaption = FormatCredentialType(credential.Type),
                Identity = credential.Ident,
                Value = kind == CredentialKind.Token ? credential.Value : String.Empty,
                Kind = kind,
                AuthUI = auther == null ? null : auther.GetUI()
            };
        }

        public virtual async Task RemoveCredential(User user, Credential cred)
        {
            await Auditing.SaveAuditRecord(new UserAuditRecord(user, UserAuditAction.RemovedCredential, cred));
            Current.DB.Credentials.Delete(cred.Id);
        }

        public async virtual Task<AuthenticateExternalLoginResult> ReadExternalLoginCredential(IOwinContext context)
        {
            var result = await context.Authentication.AuthenticateAsync(AuthenticationTypes.External);
            if (result == null)
            {
                Trace.Information("No external login found.");
                return new AuthenticateExternalLoginResult();
            }
            var idClaim = result.Identity.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
            {
                Trace.Error("External Authentication is missing required claim: " + ClaimTypes.NameIdentifier);
                return new AuthenticateExternalLoginResult();
            }

            var nameClaim = result.Identity.FindFirst(ClaimTypes.Name);
            if (nameClaim == null)
            {
                Trace.Error("External Authentication is missing required claim: " + ClaimTypes.Name);
                return new AuthenticateExternalLoginResult();
            }

            var emailClaim = result.Identity.FindFirst(ClaimTypes.Email);
            string emailSuffix = emailClaim == null ? String.Empty : (" <" + emailClaim.Value + ">");

            Authenticator auther;
            if (!Authenticators.TryGetValue(idClaim.Issuer, out auther))
            {
                auther = null;
            }

            return new AuthenticateExternalLoginResult
            {
                Authentication = null,
                ExternalIdentity = result.Identity,
                Authenticator = auther,
                Credential = CredentialBuilder.CreateExternalCredential(idClaim.Issuer, idClaim.Value, nameClaim.Value + emailSuffix)
            };
        }

        public async virtual Task<AuthenticateExternalLoginResult> AuthenticateExternalLogin(IOwinContext context)
        {
            var result = await ReadExternalLoginCredential(context);

            // Authenticate!
            if (result.Credential != null)
            {
                result.Authentication = Authenticate(result.Credential);
            }

            return result;
        }

        public static ClaimsIdentity CreateIdentity(User user, string authenticationType, params Claim[] additionalClaims)
        {
            var claims = new[] {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Username),
                new Claim(ClaimTypes.AuthenticationMethod, authenticationType),

                // Needed for anti-forgery token, also good practice to have a unique identifier claim
                new Claim(ClaimTypes.NameIdentifier, user.Username)
            }.Concat(user.Roles.Select(r => new Claim(ClaimsIdentity.DefaultRoleClaimType, r.Name)));

            if (additionalClaims.Length > 0)
            {
                claims = claims.Concat(additionalClaims);
            }

            var identity = new ClaimsIdentity(
                claims,
                authenticationType,
                nameType: ClaimsIdentity.DefaultNameClaimType,
                roleType: ClaimsIdentity.DefaultRoleClaimType);
            return identity;
        }

        private async Task ReplaceCredentialInternal(User user, Credential credential)
        {
            // Find the credentials we're replacing, if any
            var toRemove = user.Credentials
                .Where(cred =>
                    // If we're replacing a password credential, remove All password credentials
                    (credential.Type.StartsWith(CredentialTypes.Password.Prefix, StringComparison.OrdinalIgnoreCase) &&
                     cred.Type.StartsWith(CredentialTypes.Password.Prefix, StringComparison.OrdinalIgnoreCase)) ||
                    cred.Type == credential.Type)
                .ToList();

            foreach (var cred in toRemove) {
                Current.DB.Credentials.Delete(cred.Id);
            }

            if (toRemove.Any())
            {
                await Auditing.SaveAuditRecord(new UserAuditRecord(
                    user, UserAuditAction.RemovedCredential, toRemove));
            }

            user.Credentials.Add(credential);
            Current.DB.Credentials.Insert(credential);

            await Auditing.SaveAuditRecord(new UserAuditRecord(user, UserAuditAction.AddedCredential, credential));
        }

        private static CredentialKind GetCredentialKind(string type)
        {
            if (type.StartsWith("apikey", StringComparison.OrdinalIgnoreCase))
            {
                return CredentialKind.Token;
            }
            if (type.StartsWith("password", StringComparison.OrdinalIgnoreCase))
            {
                return CredentialKind.Password;
            }
            return CredentialKind.External;
        }

        private string FormatCredentialType(string credentialType)
        {
            string[] splitted = credentialType.Split('.');
            if (splitted.Length < 2)
            {
                return credentialType;
            }
            string prefix = splitted[0];
            string subtype = credentialType.Substring(splitted[0].Length + 1);

            Func<string, string> formatter;
            if (!_credentialFormatters.TryGetValue(prefix, out formatter))
            {
                return credentialType;
            }
            return formatter(subtype);
        }

        private string FormatExternalCredentialType(string externalType)
        {
            Authenticator auther;
            if (!Authenticators.TryGetValue(externalType, out auther))
            {
                return externalType;
            }
            var ui = auther.GetUI();
            return ui == null ? auther.Name : ui.Caption;
        }

        private Credential FindMatchingCredential(Credential credential)
        {
            const string sql = "select * from Credentials where Type = @type and Value = @value";
            var results = Current.DB.Query<Credential>(sql, new
            {
                @type = credential.Type, 
                @value = credential.Value
            }).ToList();

            if (results.Count == 0)
            {
                return null;
            }
            if (results.Count == 1)
            {
                return results[0];
            }
            
            // Don't put the credential itself in trace, but do put the Key for lookup later.
            string message = String.Format(
                CultureInfo.CurrentCulture,
                Strings.MultipleMatchingCredentials,
                credential.Type,
                results.First().Id);
            Trace.Error(message);
            throw new InvalidOperationException(message);
        }

        private User FindByUserNameOrEmail(string userNameOrEmail)
        {
            var users = UserService.GetUsers();
            var user = users.SingleOrDefault(u => u.Username == userNameOrEmail);
            
            if (user != null) return user;

            var allMatches = users
                .Where(u => u.EmailAddress == userNameOrEmail)
                .Take(2)
                .ToList();

            if (allMatches.Count == 1)
            {
                user = allMatches[0];
            }
            else
            {
                // If multiple matches, leave it null to signal no unique email address
                Trace.Warning("Multiple user accounts with email address: " + userNameOrEmail + " found: " + String.Join(", ", allMatches.Select(u => u.Username)));
            }
            return user;
        }

        public static bool ValidatePasswordCredential(IEnumerable<Credential> creds, string password, out Credential matched)
        {
            matched = creds.FirstOrDefault(c => ValidatePasswordCredential(c, password));
            return matched != null;
        }

        private static readonly Dictionary<string, Func<string, Credential, bool>> _validators = new Dictionary<string, Func<string, Credential, bool>>(StringComparer.OrdinalIgnoreCase) {
            { CredentialTypes.Password.Pbkdf2, (password, cred) => CryptographyService.ValidateSaltedHash(cred.Value, password, Constants.PBKDF2HashAlgorithmId) },
            { CredentialTypes.Password.Sha1, (password, cred) => CryptographyService.ValidateSaltedHash(cred.Value, password, Constants.Sha1HashAlgorithmId) }
        };

        public static bool ValidatePasswordCredential(Credential cred, string password)
        {
            Func<string, Credential, bool> validator;
            if (!_validators.TryGetValue(cred.Type, out validator))
            {
                return false;
            }
            return validator(password, cred);
        }

        private async Task MigrateCredentials(User user, List<Credential> creds, string password)
        {
            var toRemove = creds.Where(c =>
                !String.Equals(
                    c.Type,
                    CredentialTypes.Password.Pbkdf2,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Remove any non PBKDF2 credentials
            foreach (var cred in toRemove)
            {
                creds.Remove(cred);
                Current.DB.Credentials.Delete(cred.Id);
            }
            await Auditing.SaveAuditRecord(new UserAuditRecord(user, UserAuditAction.RemovedCredential, toRemove));

            // Now add one if there are no credentials left
            if (creds.Count == 0)
            {
                var newCred = CredentialBuilder.CreatePbkdf2Password(password);
                await Auditing.SaveAuditRecord(new UserAuditRecord(user, UserAuditAction.AddedCredential, newCred));
                user.Credentials.Add(newCred);
            }
        }
    }

    public class AuthenticateExternalLoginResult
    {
        public AuthenticatedUser Authentication { get; set; }
        public ClaimsIdentity ExternalIdentity { get; set; }
        public Authenticator Authenticator { get; set; }
        public Credential Credential { get; set; }
    }
}