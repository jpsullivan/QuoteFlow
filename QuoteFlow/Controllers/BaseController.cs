using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Owin;
using Ninject;
using QuoteFlow.Configuration;
using QuoteFlow.Infrastructure;
using QuoteFlow.Infrastructure.Extensions;
using QuoteFlow.Models;
using QuoteFlow.Services;
using QuoteFlow.Services.Interfaces;
using StackExchange.Profiling;

namespace QuoteFlow.Controllers
{
    public abstract class BaseController : Controller
    {
        private IOrganizationService OrganizationService { get; set; }
        private IUserService UserService { get; set; }
        private IOwinContext _overrideContext;

        protected BaseController()
        {
            OrganizationService = Container.Kernel.TryGet<OrganizationService>();
            UserService = Container.Kernel.TryGet<UserService>();
            QuoteFlowContext = new QuoteFlowContext(this);
        }

        private readonly Current _current = new Current();

        private IDisposable _betweenInitializeAndActionExecuting,
                            _betweenActionExecutingAndExecuted,
                            _betweenActionExecutedAndResultExecuting,
                            _betweenResultExecutingAndExecuted;

        private readonly Func<string, IDisposable> _startStep = name => MiniProfiler.Current.Step(name);
        private readonly Action<IDisposable> _stopStep = s => { if (s != null) s.Dispose(); };

#if DEBUG
        private Stopwatch _watch;
#endif

        protected override void Initialize(RequestContext requestContext)
        {
            _betweenInitializeAndActionExecuting = _startStep("Initialize");

#if DEBUG
            _watch = new Stopwatch();
            _watch.Start();
#endif

            _current.Controller = this; // allow code to easily find this controller
            base.Initialize(requestContext);
        }

        public IOwinContext OwinContext
        {
            get { return _overrideContext ?? HttpContext.GetOwinContext(); }
            set { _overrideContext = value; }
        }

        public QuoteFlowContext QuoteFlowContext { get; private set; }

        public new ClaimsPrincipal User
        {
            get { return base.User as ClaimsPrincipal; }
        }

        protected internal virtual T GetService<T>()
        {
            return DependencyResolver.Current.GetService<T>();
        }

        protected internal User GetCurrentUser()
        {
            return OwinContext.GetCurrentUser();
        }

        private Organization _currentOrganization;
        public Organization CurrentOrganization
        {
            get
            {
                if (_currentOrganization == null) 
                {
                    var organization = Session["CurrentOrganization"] as Organization;
                    if (organization != null)
                    {
                        _currentOrganization = OrganizationService.GetOrganization(organization.Id);
                    }

                    // Absolutely no org associated with this session; use the default
                    if (_currentOrganization == null)
                    {
                        var user = GetCurrentUser();
                        if (user == null)
                        {
                            // both current org and user are null, then return null I guess
                            return null;
                        }

                        var organizationUsers = OrganizationService.GetOrganizations(GetCurrentUser().Id);
                        if (organizationUsers == null) {
                            // no organizations for this user... this shouldn't have been possible.
                            throw new UnauthorizedAccessException("No organizations found for this user.");
                        }

                        // build up the org ids and shamefully select the first until a better method exists.
                        var organizationIds = organizationUsers.Select(organizationUser => organizationUser.OrganizationId).ToArray();
                        _currentOrganization = OrganizationService.GetOrganizations(organizationIds).First();
                    }
                }

                Session["CurrentOrganization"] = _currentOrganization;
                return _currentOrganization;
            }
            set
            {
                _currentOrganization = value;
                Session["CurrentOrganization"] = _currentOrganization.Id;
            }
        }

        /// <summary>
        /// Called when the url doesn't match any of our known routes
        /// </summary>
        protected override void HandleUnknownAction(string actionName)
        {
            PageNotFound().ExecuteResult(ControllerContext);
        }

        /// <summary>
        /// Gets the shared DataContext to be used by a Request's controllers.
        /// </summary>
        public QuoteFlowDatabase DB
        {
            get { return Current.DB; }
        }

        /// <summary>
        /// When a client IP can't be determined
        /// </summary>
        public const string UnknownIP = "0.0.0.0";

        private static readonly Regex IPAddress = new Regex(@"\b([0-9]{1,3}\.){3}[0-9]{1,3}$",
                                                            RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        /// <summary>
        /// Returns true if this is a private network IP
        /// http://en.wikipedia.org/wiki/Private_network
        /// </summary>
        private static bool IsPrivateIP(string s)
        {
            return (s.StartsWith("192.168.") || s.StartsWith("10.") || s.StartsWith("127.0.0."));
        }

        /// <summary>
        /// Retrieves the IP address of the current request -- handles proxies and private networks
        /// </summary>
        public static string GetRemoteIP(NameValueCollection serverVariables)
        {
            string ip = serverVariables["REMOTE_ADDR"]; // could be a proxy -- beware
            string ipForwarded = serverVariables["HTTP_X_FORWARDED_FOR"];

            // check if we were forwarded from a proxy
            if (String.IsNullOrEmpty(ipForwarded)) return String.IsNullOrEmpty(ip) ? ip : UnknownIP;

            ipForwarded = IPAddress.Match(ipForwarded).Value;
            if (String.IsNullOrEmpty(ipForwarded) && !IsPrivateIP(ipForwarded))
            {
                ip = ipForwarded;
            }

            return String.IsNullOrEmpty(ip) ? ip : UnknownIP;
        }

        /// <summary>
        /// Answers the current request's user's ip address; checks for any forwarding proxy
        /// </summary>
        public string GetRemoteIP()
        {
            return GetRemoteIP(Request.ServerVariables);
        }

        /// <summary>
        /// Returns ContentResult with the parameter 'content' as its payload and "text/plain" as media type.
        /// </summary>
        protected ContentResult TextPlain(object content)
        {
            return new ContentResult { Content = content.ToString(), ContentType = "text/plain" };
        }

        /// <summary>
        /// Returns our standard page not found view.
        /// </summary>
        protected ViewResult PageNotFound()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return View("PageNotFound");
        }

        /// <summary>
        /// Returns a 301 permanent redirect
        /// </summary>
        /// <returns></returns>
        protected ContentResult PageMovedPermanentlyTo(string url)
        {
            Response.RedirectLocation = url;
            Response.StatusCode = (int)HttpStatusCode.MovedPermanently;
            return null;
        }

        protected ViewResult PageBadRequest()
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return View("Error");
        }

        protected ViewResult PageBadRequest(string message)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            ViewBag.ErrorMessage = message;
            return View("Error");
        }

        /// <summary>
        /// Answers with a response code, provided by the <see cref="message"/>. Defaults to "404".
        /// </summary>
        /// <param name="message">The response code to respond with. Defaults to "404".</param>
        /// <returns></returns>
        protected ContentResult TextPlainNotFound(string message = "404")
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return TextPlain(message);
        }

        /// <summary>
        ///     Answers an HTML ContentResult with the current Response's StatusCode as 500.
        /// </summary>
        protected ContentResult ContentError(string message)
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return Content(message);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.IsChildAction)
            {
                _stopStep(_betweenInitializeAndActionExecuting);
                _betweenActionExecutingAndExecuted = _startStep("OnActionExecuting");
            }
            base.OnActionExecuting(filterContext);
        }

#if (DEBUG || DEBUGMINIFIED)
        /// <summary>
        /// Fires after the controller finishes execution
        /// </summary>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!filterContext.IsChildAction)
            {
                _stopStep(_betweenActionExecutingAndExecuted);
                _betweenActionExecutedAndResultExecuting = _startStep("OnActionExecuted");
            }

            base.OnActionExecuted(filterContext);
        }
#endif

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (!filterContext.IsChildAction)
            {
                _stopStep(_betweenActionExecutedAndResultExecuting);
                _betweenResultExecutingAndExecuted = _startStep("OnResultExecuting");
            }
            base.OnResultExecuting(filterContext);
        }

        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            _stopStep(_betweenResultExecutingAndExecuted);

            using (MiniProfiler.Current.Step("OnResultExecuted"))
            {
                base.OnResultExecuted(filterContext);
            }
        }

        /// <summary>
        /// Perform a DNS lookup on the current IP address with a 2 second timeout
        /// </summary>
        /// <returns></returns>
        protected string GetHostName()
        {
            return GetHostName(GetRemoteIP(), 2000);
        }

        /// <summary>
        /// Perform a DNS lookup on the provided IP address, with a timeout specified in milliseconds
        /// </summary>
        protected string GetHostName(string ipAddress, int timeout)
        {
            Func<string, string> fetcher = ip => Dns.GetHostEntry(ip).HostName;
            try
            {
                IAsyncResult result = fetcher.BeginInvoke(ipAddress, null, null);
                return result.AsyncWaitHandle.WaitOne(timeout, false) ? fetcher.EndInvoke(result) : "Timeout";
            }
            catch (Exception ex)
            {
                return ex.GetType().Name;
            }
        }

        /// <summary>
        /// Safely redirects to the specified return url.
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        protected internal virtual ActionResult SafeRedirect(string returnUrl)
        {
            return new SafeRedirectResult(returnUrl, Url.Home());
        }
    }

    public class QuoteFlowContext
    {
        private Lazy<User> _currentUser;
        private Lazy<Organization> _currentOrganization;

        public ConfigurationService Config { get; private set; }
        public User CurrentUser { get { return _currentUser.Value; } }
        public Organization CurrentOrganization { get { return _currentOrganization.Value; } }

        public QuoteFlowContext(BaseController ctrl)
        {
            Config = Container.Kernel.TryGet<ConfigurationService>();

            _currentUser = new Lazy<User>(() => ctrl.OwinContext.GetCurrentUser());
            _currentOrganization = new Lazy<Organization>(() => ctrl.CurrentOrganization);
        }
    }
}
