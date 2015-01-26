using System;

namespace QuoteFlow.Api.Infrastructure.Extensions
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
        /// Seperates text based on the specific seperator index.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public static string Seperate(this string s, string seperator)
        {
            int index = s.IndexOf(seperator, StringComparison.Ordinal);
            return index > 0 ? s.Substring(0, index) : "";
        }
    }
}