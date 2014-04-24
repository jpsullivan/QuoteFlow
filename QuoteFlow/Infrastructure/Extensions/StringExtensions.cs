using System;
using System.Web.Mvc;

namespace QuoteFlow.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Answers true if this String is neither null or empty.
        /// </summary>
        /// <remarks>I'm also tired of typing !String.IsNullOrEmpty(s)</remarks>
        public static bool HasValue(this string s)
        {
            return !string.IsNullOrEmpty(s);
        }

        /// <summary>
        /// Answers true if this String is either null or empty.
        /// </summary>
        /// <remarks>I'm so tired of typing String.IsNullOrEmpty(s)</remarks>
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        /// <summary>
        /// Safely converts an object to a string. If fails, returns an empty string.
        /// </summary>
        /// <param name="obj">The object which should be converted to a string.</param>
        /// <returns>A string...</returns>
        public static string ToStringSafe(this object obj)
        {
            return obj != null ? obj.ToString() : String.Empty;
        }

        /// <summary>
        /// Converts an object to a string if possible, otherwise returns null.
        /// </summary>
        /// <param name="obj">The object to try to convert to a string.</param>
        /// <returns></returns>
        public static string ToStringOrNull(this object obj)
        {
            return obj == null ? null : obj.ToString();
        }

        /// <summary>
        /// Produces a URL-friendly version of this String, "like-this-one".
        /// </summary>
        public static string UrlFriendly(this string s)
        {
            return s.HasValue() ? UrlHelpers.UrlFriendly(s) : s;
        }
    }
}