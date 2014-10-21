using System.Collections.Generic;

namespace QuoteFlow.Models.Search.Jql.Query.Order
{
    /// <summary>
    /// Represents the ordering portion of the a search query. The results can be sorted by fields in either a
    /// <seealso cref="SortOrder#ASC ascending"/> or
    /// <seealso cref="SortOrder#DESC descending"/> order. The actual sort is made up of a list of
    /// (field, order) pair(s). Each of the pair is represented by a <seealso cref="SearchSort"/> object.
    /// </summary>
    public interface IOrderBy
    {
        /// <returns>
        /// A list of SearchSort objects that represent the specified sorting requested for this OrderBy clause. 
        /// Cannot be null. 
        /// </returns>
        IList<SearchSort> SearchSorts { get; }
    }

}