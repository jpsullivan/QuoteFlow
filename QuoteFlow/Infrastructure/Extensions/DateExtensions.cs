using System;
using Microsoft.Ajax.Utilities;

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
            return date != null && date.Value.IsInThePast();
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

        /// <summary>
        /// returns a html span element with relative time elapsed since this event occurred, eg, "3 months ago" or "yesterday"; 
        /// assumes time is *already* stored in UTC format!
        /// </summary>
        public static string ToRelativeTimeSpan(this DateTime dt, string cssclass = "relativetime")
        {
            if (cssclass == null) 
            {
                return string.Format(@"<span title=""{0:G} -- {2:u}"">{1}</span>", dt, ToRelativeTime(dt), dt.ToUniversalTime());
            }

            return string.Format(@"<span title=""{0:G} -- {3:u}"" class=""{2}"">{1}</span>", dt, ToRelativeTime(dt), cssclass, dt.ToUniversalTime());
        }

        /// <summary>
        /// Returns a humanized string indicating how long ago something happened, eg "3 days ago".
        /// For future dates, returns when this DateTime will occur from DateTime.UtcNow.
        /// </summary>
        public static string ToRelativeTime(this DateTime dt)
        {
            DateTime utcNow = DateTime.Now;
            return dt <= utcNow ? ToRelativeTimePast(dt, utcNow) : ToRelativeTimeFuture(dt, utcNow);
        }

        private static string ToRelativeTimePast(DateTime dt, DateTime utcNow)
        {
            TimeSpan ts = utcNow - dt;
            double delta = ts.TotalSeconds;

            if (delta < 60)
            {
                return ts.Seconds == 1 ? "1 sec ago" : ts.Seconds + " secs ago";
            }
            if (delta < 3600) // 60 mins * 60 sec
            {
                return ts.Minutes == 1 ? "1 min ago" : ts.Minutes + " mins ago";
            }
            if (delta < 86400)  // 24 hrs * 60 mins * 60 sec
            {
                return ts.Hours == 1 ? "1 hour ago" : ts.Hours + " hours ago";
            }

            int days = ts.Days;
            if (days == 1)
            {
                return "yesterday";
            }
            if (days <= 2)
            {
                return days + " days ago";
            }
            if (days <= 330)
            {
                return dt.ToString("MMM %d 'at' %H:mmm tt");
            }
            return dt.ToString(@"MMM %d \'yy 'at' %H:mmm tt");
        }

        private static string ToRelativeTimeFuture(DateTime dt, DateTime utcNow)
        {
            TimeSpan ts = dt - utcNow;
            double delta = ts.TotalSeconds;

            if (delta < 60)
            {
                return ts.Seconds == 1 ? "in 1 second" : "in " + ts.Seconds + " seconds";
            }
            if (delta < 3600) // 60 mins * 60 sec
            {
                return ts.Minutes == 1 ? "in 1 minute" : "in " + ts.Minutes + " minutes";
            }
            if (delta < 86400) // 24 hrs * 60 mins * 60 sec
            {
                return ts.Hours == 1 ? "in 1 hour" : "in " + ts.Hours + " hours";
            }

            // use our own rounding so we can round the correct direction for future
            int days = (int)Math.Round(ts.TotalDays, 0);
            if (days == 1)
            {
                return "tomorrow";
            }
            if (days <= 10)
            {
                return "in " + days + " day" + (days > 1 ? "s" : "");
            }
            if (days <= 330)
            {
                return "on " + dt.ToString("MMM %d 'at' %H:mmm");
            }
            return "on " + dt.ToString(@"MMM %d \'yy 'at' %H:mmm");
        }

        /// <summary>
        /// Get a duration string with the possibility of a negative. A duration will be
        /// considered negative if the first non-space character is a - sign.
        /// </summary>
        /// <param name="duration">The duration string</param>
        /// <returns>The duration in seconds, which can be negative.</returns>
        public static long ToDurationWithNegative(string duration)
        {
            var cleanedDuration = duration.Trim();
            if (cleanedDuration.IsNullOrEmpty())
            {
                return 0;
            }

            bool negative = cleanedDuration[0] == '-';

            if (negative)
            {
//                return 0 - getDuration(cleanedDurationStr.Substring(1));
            }

//            return getDuration(cleanedDurationStr);
            return 0;
        }
    }
}