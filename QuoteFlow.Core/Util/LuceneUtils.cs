using System;
using System.Globalization;
using Lucene.Net.Util;

namespace QuoteFlow.Core.Util
{
    /// <summary>
    /// A simple utility class for our common Lucene usage methods.
    /// </summary>
    public class LuceneUtils
    {
        private const string LOCALDATE_MAX_VALUE = "99999999";

        /// <summary>
        /// do not construct
        /// </summary>
        private LuceneUtils()
        {
        }

//        /// <summary>
//        /// Turns a given <seealso cref="LocalDate"/> value into a String suitable for storing and searching in Lucene.
//        /// <p>
//        /// The date  is stored as "YYYYMMDD".  If the date is null we store "99999999"
//        /// which causes nulls to sort to the end.  (This is traditional JIRA behaviour)
//        /// </summary>
//        /// <param name="localDate"> the date to be converted.  May be null </param>
//        /// <returns> a string representing the date </returns>
//        public static string localDateToString(LocalDate localDate)
//        {
//            if (localDate == null)
//            {
//                return LOCALDATE_MAX_VALUE;
//            }
//            return LocalDateFactory.toIsoBasic(localDate);
//        }
//
//        public static LocalDate stringToLocalDate(string indexValue)
//        {
//            if (indexValue == null || indexValue.Equals(LOCALDATE_MAX_VALUE))
//            {
//                return null;
//            }
//            return LocalDateFactory.fromIsoBasicFormat(indexValue);
//        }

        /// <summary>
        /// Turns a given date-time (point in time) value into a String suitable for storing and searching in Lucene.
        /// 
        /// The date-time is stored as the number of seconds.  If the date is null we store the encoded form of Long.MAX_VALUE
        /// which causes nulls to sort to the end.  (This is traditional JIRA behaviour)
        /// </summary>
        /// <param name="date"> the date to be converted.  May be null </param>
        /// <returns> a string representing the number of seconds </returns>
        public static string DateToString(DateTime date)
        {
            return date.Ticks.ToString(CultureInfo.InvariantCulture);
        }

        public static DateTime? StringToDate(string s)
        {
            if (s != null)
            {
                long seconds = NumericUtils.PrefixCodedToLong(s);
                if (seconds == long.MaxValue)
                {
                    return null;
                }
                return new DateTime(seconds * 1000);
            }
            return DateTime.Now;
        }

    }
}