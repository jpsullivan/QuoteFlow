using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteFlow.Infrastructure.Extensions
{
    public static class DateExtensions
    {
        /// <summary>
        /// Answers true if the nullable DateTime object is older than now.
        /// </summary>
        /// <param name="date">The date to check if is older than right now.</param>
        /// <returns>True if is in the past.</returns>
        public static bool IsInThePast(this DateTime? date)
        {
            return date.Value.IsInThePast();
        }

        /// <summary>
        /// Answers true if the DateTime object is older than now.
        /// </summary>
        /// <param name="date">The date to check if is older than right now.</param>
        /// <returns>True if is in the past.</returns>
        public static bool IsInThePast(this DateTime date)
        {
            return date < DateTime.UtcNow;
        }
    }
}