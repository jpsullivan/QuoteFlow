﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Models.ViewModels;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Authentication;
using QuoteFlow.Core.Infrastructure.Auth;
using QuoteFlow.Core.Infrastructure.Exceptions;
using QuoteFlow.Infrastructure.Attributes;
using QuoteFlow.Infrastructure.Extensions;

namespace QuoteFlow.Controllers
{
    public partial class AuthenticationController : AppController
    {
        #region IoC

        public AuthenticationService AuthService { get; protected set; }
        public IUserService UserService { get; protected set; }
        public IMessageService MessageService { get; protected set; }

        // For sub-classes to initialize services themselves
        protected AuthenticationController()
        {
        }

        public AuthenticationController(
            AuthenticationService authService,
            IUserService userService,
            IMessageService messageService)
        {
            AuthService = authService;
            UserService = userService;
            MessageService = messageService;
        }

        #endregion

        /// <summary>
        /// Sign In\Register view
        /// </summary>
        [RequireSsl]
        [QuoteFlowRoute("account/logon")]
        public virtual ActionResult LogOn(string returnUrl)
        {
            // I think it should be obvious why we don't want the current URL to be the return URL here ;)
            ViewData[Constants.ReturnUrlViewDataKey] = returnUrl;

            if (Request.IsAuthenticated)
            {
                TempData["Message"] = Strings.AlreadyLoggedIn;
                return SafeRedirect(returnUrl);
            }

            return LogOnView();
        }

        [RequireSsl]
        [ValidateAntiForgeryToken]
        [QuoteFlowRoute("account/signin"), AcceptVerbs(HttpVerbs.Post)]
        public virtual async Task<ActionResult> SignIn(LogOnViewModel model, string returnUrl, bool linkingAccount)
        {
            // I think it should be obvious why we don't want the current URL to be the return URL here ;)
            ViewData[Constants.ReturnUrlViewDataKey] = returnUrl;

            if (Request.IsAuthenticated)
            {
                TempData["Message"] = Strings.AlreadyLoggedIn;
                return SafeRedirect(returnUrl);
            }

            if (!ModelState.IsValid)
            {
                return LogOnView(model);
            }

            var user = await AuthService.Authenticate(model.SignIn.UserNameOrEmail, model.SignIn.Password);

            if (user == null)
            {
                ModelState.AddModelError("SignIn", Strings.UsernameAndPasswordNotFound);
                return LogOnView(model);
            }

            if (linkingAccount)
            {
                // Link with an external account
                user = await AssociateCredential(user, returnUrl);
                if (user == null)
                {
                    return ExternalLinkExpired();
                }
            }

            // Now log in!
            AuthService.CreateSession(OwinContext, user.User);
            return SafeRedirect(returnUrl);
        }

        [RequireSsl]
        [QuoteFlowRoute("account/register"), AcceptVerbs(HttpVerbs.Get)]
        public virtual ActionResult Register(string returnUrl) 
        {
            // I think it should be obvious why we don't want the current URL to be the return URL here ;)
            ViewData[Constants.ReturnUrlViewDataKey] = returnUrl;

            if (Request.IsAuthenticated)
            {
                TempData["Message"] = Strings.AlreadyLoggedIn;
                return SafeRedirect(returnUrl);
            }

            return RegisterView();
        }

        [RequireSsl]
        [ValidateAntiForgeryToken]
        [QuoteFlowRoute("account/registeruser"), AcceptVerbs(HttpVerbs.Post)]
        public async virtual Task<ActionResult> RegisterUser(LogOnViewModel model, string returnUrl, bool linkingAccount)
        {
            // I think it should be obvious why we don't want the current URL to be the return URL here ;)
            ViewData[Constants.ReturnUrlViewDataKey] = returnUrl;

            if (Request.IsAuthenticated)
            {
                TempData["Message"] = Strings.AlreadyLoggedIn;
                return SafeRedirect(returnUrl);
            }

            if (linkingAccount)
            {
                ModelState.Remove("Register.Password");
            }

            if (!ModelState.IsValid)
            {
                return RegisterView(model);
            }

            AuthenticatedUser user;
            try
            {
                if (linkingAccount)
                {
                    var result = await AuthService.ReadExternalLoginCredential(OwinContext);
                    if (result.ExternalIdentity == null)
                    {
                        return ExternalLinkExpired();
                    }

                    user = await AuthService.Register(
                        model.Register.Username,
                        model.Register.EmailAddress,
                        model.Register.FullName,
                        result.Credential);
                }
                else
                {
                    user = await AuthService.Register(
                        model.Register.Username,
                        model.Register.EmailAddress,
                        model.Register.FullName,
                        CredentialBuilder.CreatePbkdf2Password(model.Register.Password));
                }
            }
            catch (EntityException ex)
            {
                ModelState.AddModelError("Register", ex.Message);
                return LogOnView(model);
            }

            // Send a new account email
            if (QuoteFlowContext.Config.Current.ConfirmEmailAddresses && user.User.UnconfirmedEmailAddress.HasValue())
            {
                MessageService.SendNewAccountEmail(
                    new MailAddress(user.User.UnconfirmedEmailAddress, user.User.Username),
                    Url.ConfirmationUrl(
                        "Confirm",
                        "Users",
                        user.User.Username,
                        user.User.EmailConfirmationToken));
            }

            // We're logging in!
            AuthService.CreateSession(OwinContext, user.User);

            return RedirectFromRegister(returnUrl);
        }

        [QuoteFlowRoute("account/logoff")]
        public virtual ActionResult LogOff(string returnUrl)
        {
            OwinContext.Authentication.SignOut();
            return SafeRedirect(returnUrl);
        }

        [QuoteFlowRoute("users/account/authenticate/{provider}")]
        public virtual ActionResult Authenticate(string returnUrl, string provider)
        {
            return AuthService.Challenge(
                provider,
                Url.Action("LinkExternalAccount", "Authentication", new { ReturnUrl = returnUrl }));
        }

        [QuoteFlowRoute("users/account/authenticate/return")]
        public async virtual Task<ActionResult> LinkExternalAccount(string returnUrl)
        {
            // Extract the external login info
            var result = await AuthService.AuthenticateExternalLogin(OwinContext);
            if (result.ExternalIdentity == null)
            {
                // User got here without an external login cookie (or an expired one)
                // Send them to the logon action
                return ExternalLinkExpired();
            }

            if (result.Authentication != null)
            {
                AuthService.CreateSession(OwinContext, result.Authentication.User);
                return SafeRedirect(returnUrl);
            }
            else
            {
                // Gather data for view model
                var authUI = result.Authenticator.GetUI();
                var email = result.ExternalIdentity.GetClaimOrDefault(ClaimTypes.Email);
                var name = result
                    .ExternalIdentity
                    .GetClaimOrDefault(ClaimTypes.Name);

                // Check for a user with this email address
                User existingUser = null;
                if (!String.IsNullOrEmpty(email))
                {
                    existingUser = UserService.GetUser(email);
                }

                var external = new AssociateExternalAccountViewModel()
                {
                    ProviderAccountNoun = authUI.AccountNoun,
                    AccountName = name,
                    FoundExistingUser = existingUser != null
                };

                var model = new LogOnViewModel()
                {
                    External = external,
                    SignIn = new SignInViewModel()
                    {
                        UserNameOrEmail = email
                    },
                    Register = new RegisterViewModel()
                    {
                        EmailAddress = email
                    }
                };

                return LogOnView(model);
            }
        }

        private ActionResult RedirectFromRegister(string returnUrl)
        {
            if (returnUrl != Url.Home())
            {
                // User was on their way to a page other than the home page. Redirect them with a thank you for registering message.
                TempData["Message"] = "Your account is now registered!";
                return SafeRedirect(returnUrl);
            }

            // User was not on their way anywhere in particular. Show them the thanks/welcome page.
            return RedirectToAction("Thanks", "Users");
        }

        private async Task<AuthenticatedUser> AssociateCredential(AuthenticatedUser user, string returnUrl)
        {
            var result = await AuthService.ReadExternalLoginCredential(OwinContext);
            if (result.ExternalIdentity == null)
            {
                // User got here without an external login cookie (or an expired one)
                // Send them to the logon action
                return null;
            }

            await AuthService.AddCredential(user.User, result.Credential);

            // Notify the user of the change
            MessageService.SendCredentialAddedNotice(user.User, result.Credential);

            return new AuthenticatedUser(user.User, result.Credential);
        }

        private List<AuthenticationProviderViewModel> GetProviders()
        {
            return (from p in AuthService.Authenticators.Values
                    where p.BaseConfig.Enabled
                    let ui = p.GetUI()
                    where ui != null
                    select new AuthenticationProviderViewModel()
                    {
                        ProviderName = p.Name,
                        UI = ui
                    }).ToList();
        }

        private ActionResult ExternalLinkExpired()
        {
            // User got here without an external login cookie (or an expired one)
            // Send them to the logon action with a message
            TempData["Message"] = Strings.ExternalAccountLinkExpired;
            return RedirectToAction("LogOn");
        }

        private ActionResult LogOnView()
        {
            return LogOnView(new LogOnViewModel {
                SignIn = new SignInViewModel(),
                Register = new RegisterViewModel()
            });
        }

        private ActionResult LogOnView(LogOnViewModel existingModel)
        {
            // Fill the providers list
            existingModel.Providers = GetProviders();

            // Reinitialize any nulled-out sub models
            existingModel.SignIn = existingModel.SignIn ?? new SignInViewModel();
            existingModel.Register = existingModel.Register ?? new RegisterViewModel();

            return View("LogOn", existingModel);
        }

        private ActionResult RegisterView() 
        {
            return RegisterView(new LogOnViewModel {
                Register = new RegisterViewModel()
            });
        }

        private ActionResult RegisterView(LogOnViewModel existingModel) 
        {
            // Fill the providers list
            existingModel.Providers = GetProviders();

            // Reinitialize any nulled-out sub models
            existingModel.Register = existingModel.Register ?? new RegisterViewModel();

            return View("Register", existingModel);
        }
    }
}
