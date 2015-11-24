using System.Collections.Generic;

namespace QuoteFlow.Api.Models.OrderBy
{
    /// <summary>
    /// The "order by" options. That is, the options that the user can
    /// click on to change the sort.
    /// </summary>
    public class OrderByOptions
    {
        /// <summary>
        /// The number of fields that match the field name query parameter.
        /// </summary>
        public int MatchesCount { get; set; }

        /// <summary>
        /// The total number of fields.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// The *effective* maximum number of options returned.
        /// </summary>
        public int MaxResults { get; set; }

        /// <summary>
        /// The fields that the user can click on.
        /// </summary>
        public IEnumerable<OrderByOption> Fields { get; set; }
    }
}