using System;

namespace QuoteFlow.Api.Jql.Util
{
    /// <summary>
    /// A simple class to represent a date range
    /// </summary>
    public sealed class DateRange
    {
        /// <returns> the lower of the dates in the date range </returns>
        public DateTime LowerDate { get; private set; }

        /// <returns> the lower of the dates in the date range </returns>
        public DateTime UpperDate { get; private set; }

        /// <summary>
        /// The passed in dates can never be null and if the lower is higher than the upper then they are swapped into lower
        /// then upper order.
        /// </summary>
        /// <param name="lowerDate"> the lower date of the range </param>
        /// <param name="upperDate"> the upper date of the range </param>
        public DateRange(DateTime lowerDate, DateTime upperDate)
        {
            if (lowerDate == null)
            {
                throw new ArgumentNullException(nameof(lowerDate));
            }

            if (upperDate == null)
            {
                throw new ArgumentNullException(nameof(upperDate));
            }

            LowerDate = MinOf(lowerDate, upperDate);
            UpperDate = MaxOf(lowerDate, upperDate);
        }

        private static DateTime MaxOf(DateTime lowerDate, DateTime upperDate)
        {
            return (lowerDate > upperDate ? lowerDate : upperDate);

        }

        private static DateTime MinOf(DateTime lowerDate, DateTime upperDate)
        {
            return (lowerDate <= upperDate ? lowerDate : upperDate);
        }
    }
}