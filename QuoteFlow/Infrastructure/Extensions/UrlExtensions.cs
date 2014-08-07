using System;
using System.Web.Mvc;
using System.Web.Routing;
using QuoteFlow.Models;

namespace QuoteFlow.Infrastructure.Extensions
{
    public static class UrlExtensions
    {
        // Shorthand for current url
        public static string Current(this UrlHelper url)
        {
            return url.RequestContext.HttpContext.Request.RawUrl;
        }

        public static string Absolute(this UrlHelper url, string path)
        {
            UriBuilder builder = GetCanonicalUrl(url);
            builder.Path = path;
            return builder.Uri.AbsoluteUri;
        }

        #region Base Pages

        public static string Home(this UrlHelper url)
        {
            return url.Action("Index", "Dashboard");
        }

        public static string GettingStarted(this UrlHelper url)
        {
            return url.Action("GettingStarted", "Dashboard");
        }

        #endregion

        #region Assets

        public static string NewAsset(this UrlHelper url)
        {
            return url.Action("New", "Asset");
        }

        public static string Asset(this UrlHelper url, int assetId, string assetName)
        {
            return string.Format("/asset/{0}/{1}", assetId, assetName.UrlFriendly());
        }

        public static string EditAsset(this UrlHelper url, int assetId, string assetName)
        {
            return string.Format("/asset/{0}/{1}/edit", assetId, assetName.UrlFriendly());
        }

        #endregion

        #region Catalogs

        public static string NewCatalog(this UrlHelper url)
        {
            return url.Action("New", "Catalog");
        }

        public static string Catalog(this UrlHelper url, int catalogId, string catalogName)
        {
            return string.Format("/catalog/{0}/{1}", catalogId, catalogName);
        }

        public static string CatalogAssets(this UrlHelper url, int catalogId, string catalogName)
        {
            return string.Format("/catalog/{0}/{1}/assets", catalogId, catalogName.UrlFriendly());
        }

        public static string CatalogAssets(this UrlHelper url, int catalogId, string catalogName, int pageNumber)
        {
            return string.Format("/catalog/{0}/{1}/assets?page={2}", catalogId, catalogName, pageNumber);
        }

        public static string CatalogAssetsInteractive(this UrlHelper url, int catalogId, string catalogName)
        {
            return string.Format("/catalog/{0}/{1}/assets/iv", catalogId, catalogName.UrlFriendly());
        }

        public static string CatalogAssetsInteractive(this UrlHelper url, int catalogId, string catalogName, int pageNumber)
        {
            return string.Format("/catalog/{0}/{1}/assets/iv?page={2}", catalogId, catalogName.UrlFriendly(), pageNumber);
        }

        public static string CatalogVersions(this UrlHelper url, int catalogId, string catalogName)
        {
            return string.Format("/catalog/{0}/{1}/versions", catalogId, catalogName);
        }

        public static string CatalogImportResults(this UrlHelper url, int catalogId, string catalogName)
        {
            return string.Format("/catalog/{0}/{1}/import-results", catalogId, catalogName.UrlFriendly());
        }

        public static string CatalogImportResults(this UrlHelper url, int catalogId, string catalogName, int pageNumber)
        {
            return string.Format("/catalog/{0}/{1}/import-results?page={2}", catalogId, catalogName.UrlFriendly(), pageNumber);
        }

        public static string CatalogImportResultsSuccess(this UrlHelper url, int catalogId, string catalogName, int pageNumber)
        {
            return string.Format("/catalog/{0}/{1}/import-results/successful?page={2}", catalogId, catalogName.UrlFriendly(), pageNumber);
        }

        public static string CatalogImportResultsSkipped(this UrlHelper url, int catalogId, string catalogName, int pageNumber)
        {
            return string.Format("/catalog/{0}/{1}/import-results/skipped?page={2}", catalogId, catalogName.UrlFriendly(), pageNumber);
        }

        public static string CatalogImportResultsFailed(this UrlHelper url, int catalogId, string catalogName, int pageNumber)
        {
            return string.Format("/catalog/{0}/{1}/import-results/failed?page={2}", catalogId, catalogName.UrlFriendly(), pageNumber);
        }

        public static string ImportCatalog(this UrlHelper url)
        {
            return url.Action("Import", "Catalog");
        }

        public static string CancelCatalogImport(this UrlHelper url)
        {
            return url.Action("CancelImport", "Catalog");
        }

        #endregion

        #region Manufacturers

        public static string Manufacturer(this UrlHelper url, int id, string manufacturerName)
        {
            return string.Format("/manufacturer/{0}/{1}", id, manufacturerName.UrlFriendly());
            return url.RouteUrl("Manufacturer-Show", new {id, name = manufacturerName.UrlFriendly()});
        }

        public static string ManufacturerAssets(this UrlHelper url, int id, string manufacturerName)
        {
            return string.Format("/manufacturer/{0}/{1}/assets", id, manufacturerName.UrlFriendly());
        }

        public static string EditManufacturer(this UrlHelper url, int id, string manufacturerName)
        {
            return string.Format("/manufacturer/{0}/{1}/edit", id, manufacturerName.UrlFriendly());
        }

        #endregion

        #region Quotes

        public static string Quote(this UrlHelper url, int id, string quoteName)
        {
            return string.Format("/quote/{0}/{1}", id, quoteName.UrlFriendly());
        }

        public static string NewQuote(this UrlHelper url)
        {
            return string.Format("/quote/new");
        }

        #endregion

        #region Users

        public static string User(this UrlHelper url, int userId, string username)
        {
            return string.Format("/user/{0}/{1}", userId, username.UrlFriendly());
        }

        #endregion

        public static string ConfirmationUrl(this UrlHelper url, string action, string controller, string username, string token)
        {
            return ConfirmationUrl(url, action, controller, username, token, null);
        }

        public static string ConfirmationUrl(this UrlHelper url, string action, string controller, string username, string token, object routeValues)
        {
            var rvd = routeValues == null ? new RouteValueDictionary() : new RouteValueDictionary(routeValues);
            rvd["username"] = username;
            rvd["token"] = token;
            return url.Action(
                action,
                controller,
                rvd,
                url.RequestContext.HttpContext.Request.Url.Scheme,
                url.RequestContext.HttpContext.Request.Url.Host);
        }

        public static string LogOn(this UrlHelper url)
        {
            return url.Action("LogOn", "Authentication");
        }

        public static string LogOn(this UrlHelper url, string returnUrl) 
        {
            return url.Action("LogOn", "Authentication", new {returnUrl = returnUrl});
        }

        public static string ConfirmationRequired(this UrlHelper url)
        {
            return url.Action("ConfirmationRequired", controllerName: "Users");
        }

        public static string LogOff(this UrlHelper url)
        {
            string returnUrl = url.Current();
            // If we're logging off from the Admin Area, don't set a return url
            if (String.Equals(url.RequestContext.RouteData.DataTokens["area"].ToStringOrNull(), "Admin", StringComparison.OrdinalIgnoreCase))
            {
                returnUrl = String.Empty;
            }
            var originalResult = MVC.Authentication.LogOff(returnUrl);
            var result = originalResult.GetT4MVCResult();

            // T4MVC doesn't set area to "", but we need it to, otherwise it thinks this is an intra-area link.
            result.RouteValueDictionary["area"] = "";

            return url.Action(originalResult);
        }

        public static string Register(this UrlHelper url, string returnUrl)
        {
            return url.Action("Register", "Authentication", new {returnUrl = returnUrl});
        }

        public static string User(this UrlHelper url, User user, string scheme = null)
        {
            string result = url.Action(MVC.Users.Profiles(user.Username), protocol: scheme);
            return EnsureTrailingSlash(result);
        }

        private static UriBuilder GetCanonicalUrl(UrlHelper url)
        {
            UriBuilder builder = new UriBuilder(url.RequestContext.HttpContext.Request.Url);
            builder.Query = String.Empty;
            if (builder.Host.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
            {
                builder.Host = builder.Host.Substring(4);
            }
            return builder;
        }

        public static string EnsureTrailingSlash(this string urlPart)
        {
            return !urlPart.EndsWith("/", StringComparison.Ordinal) ? urlPart + '/' : urlPart;
        }
    }
}