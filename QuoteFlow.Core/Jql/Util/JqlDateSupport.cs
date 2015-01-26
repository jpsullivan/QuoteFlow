using System;
using System.Text.RegularExpressions;
using QuoteFlow.Api.Jql.Util;

namespace QuoteFlow.Core.Jql.Util
{
    /// <summary>
    /// Default implementation for <see cref="IJqlDateSupport"/>
    /// </summary>
    public class JqlDateSupport : IJqlDateSupport
    {
        private const string YYYY_MM_DD1 = "yyyy/MM/dd";
		private const string YYYY_MM_DD2 = "yyyy-MM-dd";
        private const string YYYY_MM_DD_HH_MM1 = YYYY_MM_DD1 + " HH:mm";
        private const string YYYY_MM_DD_HH_MM2 = YYYY_MM_DD2 + " HH:mm";
        private const string ATLASSIAN_DURATION = "AD";
		private static readonly string[] ACCEPTED_FORMATS = {YYYY_MM_DD_HH_MM1, YYYY_MM_DD_HH_MM2, YYYY_MM_DD1, YYYY_MM_DD2};
        private static readonly Regex DURATION_PATTERN = new Regex("(?:\\d+(?:\\.\\d+)?|\\.\\d+)(.)?", RegexOptions.IgnoreCase);

        internal enum Precision
        {
            Minutes,
            Hours,
            Days
        }

        public DateTime ConvertToDate(string dateString)
        {
            throw new NotImplementedException();
        }

        public DateTime ConvertToDate(string dateString, TimeZone timeZone)
        {
            throw new NotImplementedException();
        }

        public DateRange ConvertToDateRangeWithImpliedPrecision(string dateString)
        {
            throw new NotImplementedException();
        }

        public DateTime ConvertToDate(long? dateLong)
        {
            throw new NotImplementedException();
        }

        public DateRange ConvertToDateRange(long? dateLong)
        {
            throw new NotImplementedException();
        }

        public string GetIndexedValue(DateTime date)
        {
            throw new NotImplementedException();
        }

        public bool Validate(string dateString)
        {
            throw new NotImplementedException();
        }

        public string GetDateString(DateTime date)
        {
            throw new NotImplementedException();
        }

        public string GetDateString(DateTime date, TimeZone timeZone)
        {
            throw new NotImplementedException();
        }

        public bool IsDuration(string dateString)
        {
            throw new NotImplementedException();
        }
    }
}
