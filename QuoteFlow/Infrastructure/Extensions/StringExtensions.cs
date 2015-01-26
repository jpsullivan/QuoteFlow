using System.Web.Mvc;
using QuoteFlow.Api.Infrastructure.Extensions;

namespace QuoteFlow.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Produces a URL-friendly version of this String, "like-this-one".
        /// </summary>
        public static string UrlFriendly(this string s)
        {
            return s.HasValue() ? UrlHelpers.UrlFriendly(s) : s;
        }
    }
}