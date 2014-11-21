//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//
//namespace QuoteFlow.Infrastructure.Helpers
//{
//    public static class DateHelpers
//    {
//        /// <summary>
//        /// Check whether a given duration string is valid
//        /// </summary>
//        /// <param name="s"> the duration string </param>
//        /// <returns> true if it a valid duration </returns>
//        public static bool ValidDuration(string s)
//        {
//            try
//            {
//                getDuration(s);
//                return true;
//            }
//            catch (Exception e)
//            {
//                return false;
//            }
//        }
//
//        /// <summary>
//        /// Given a duration string, get the number of seconds it represents (all case insensitive):
//        /// 
//        /// - w = weeks
//        /// - d = days
//        /// - h = hours
//        /// - m = minutes
//        /// 
//        /// If no category is specified, assume minutes. Each field must be separated by a space, 
//        /// and they can come in any order. Case is ignored.
//        /// 
//        /// ie 2h = 7200, 60m = 3600, 3d = 259200, 30m
//        /// </summary>
//        /// <param name="durationStr">The duration string.</param>
//        /// <returns>The duration in seconds.</returns>
//        public static long getDuration(string durationStr)
//        {
//            return getDuration(durationStr, Duration.MINUTE);
//        }
//
//        /// <summary>
//        /// Given a duration string, get the number of seconds it represents (all case insensitive):
//        /// <ul>
//        /// <li>w = weeks
//        /// <li>d = days
//        /// <li>h = hours
//        /// <li>m = minutes
//        /// </ul>
//        /// ie 2h = 7200, 60m = 3600, 3d = 259200, 30m
//        /// </summary>
//        /// <param name="durationStr"> the duration string </param>
//        /// <param name="defaultUnit"> the unit used when another is not specified in the durationStr </param>
//        /// <returns> the duration in seconds </returns>
//        /// <exception cref="InvalidDurationException"> if the duration is invalid </exception>
//        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//        //ORIGINAL LINE: public static long getDuration(final String durationStr, final Duration defaultUnit) throws InvalidDurationException
//        public static long getDuration(string durationStr, Duration defaultUnit)
//        {
//            return getDurationSeconds(durationStr, Duration.DAY.Seconds, Duration.WEEK.Seconds, defaultUnit);
//        }
//
//        /// <summary>
//        /// This function retrieves a duration in seconds that depends on number of hours in a day and
//        /// days in a week. The default unit is MINUTE (i.e. "2" == "2 minutes")
//        /// </summary>
//        /// <param name="durationStr"> to convert to a duration </param>
//        /// <param name="hoursPerDay"> Number of hourse i day </param>
//        /// <param name="daysPerWeek"> Days Per Week </param>
//        /// <returns> the duration in seconds </returns>
//        /// <exception cref="InvalidDurationException"> if its badly formatted duration </exception>
//        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//        //ORIGINAL LINE: public static long getDuration(String durationStr, int hoursPerDay, int daysPerWeek) throws InvalidDurationException
//        public static long getDuration(string durationStr, int hoursPerDay, int daysPerWeek)
//        {
//            return getDuration(durationStr, hoursPerDay, daysPerWeek, Duration.MINUTE);
//        }
//
//        /// <summary>
//        /// This function retrieves a duration in seconds that depends on number of hours in a day and
//        /// days in a week
//        /// </summary>
//        /// <param name="durationStr"> to convert to a duration </param>
//        /// <param name="hoursPerDay"> Number of hourse i day </param>
//        /// <param name="daysPerWeek"> Days Per Week </param>
//        /// <param name="defaultUnit"> the unit used when one is not specified on a measure in the durationStr </param>
//        /// <returns> the duration in seconds </returns>
//        /// <exception cref="InvalidDurationException"> if its badly formatted duration </exception>
//        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//        //ORIGINAL LINE: public static long getDuration(String durationStr, int hoursPerDay, int daysPerWeek, final Duration defaultUnit) throws InvalidDurationException
//        public static long getDuration(string durationStr, int hoursPerDay, int daysPerWeek, Duration defaultUnit)
//        {
//            long secondsInDay = hoursPerDay * Duration.HOUR.Seconds;
//            long secondsPerWeek = daysPerWeek * secondsInDay;
//            return getDurationSeconds(durationStr, secondsInDay, secondsPerWeek, defaultUnit);
//        }
//    }
//}