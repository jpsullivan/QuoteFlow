using System;

namespace QuoteFlow.Api.Jql.Query.Order
{
    public enum SortOrder
    {
        /// <summary>
        /// The ascending order.
        /// </summary>
        ASC,

        /// <summary>
        /// The descending order.
        /// </summary>
        DESC
    }

    public static class SortOrderHelpers
    {
        /// <summary>
        /// Find the SortOrder represented in the passed string. The matching is based on the names of the value in the
        /// enumeration. All matching is done in a case insensitive manner. The order <seealso cref="#ASC"/> will be returned if
        /// no match is found.
        /// </summary>
        /// <param name="value"> the string to parse. </param>
        /// <returns> the parsed SortOrder. </returns>
        public static SortOrder ParseString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return SortOrder.ASC;
            }

            SortOrder parsedEnum;
            return Enum.TryParse(value, true, out parsedEnum) ? parsedEnum : SortOrder.ASC;
        }
    }
}