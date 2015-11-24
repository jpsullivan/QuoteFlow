namespace QuoteFlow.Api.Models.OrderBy
{
    public class OrderByRequest
    {
        /// <summary>
        /// The JQL that we are currently sorting by (used to build the new sort JQL)
        /// </summary>
        public string Jql { get; set; }

        /// <summary>
        /// A partial field name query that we will *try* to match against. (optional)
        /// </summary>
        public string Query { get; set; }

        public string SortBy { get; set; }

        /// <summary>
        /// The maximum number of results to return. (optional)
        /// </summary>
        public int MaxResults { get; set; } = 10;
    }
}