using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Query.Order;

namespace QuoteFlow.Models.Search.Jql.Query
{
    public interface IQuery
    {
        /// <summary>
        /// Returns the main clause of the search which can be any number of nested
        /// clauses that will make up the full search query. Null indicates that no
        /// where clause is available and all issues should be returned.
        /// </summary>
        /// <returns></returns>
        IClause WhereClause();

        /// <summary>
        /// Return the sorting position of the search which can be any number of 
        /// <see cref="SearchSort"/>s that will make up the full order by clause.
        /// Null indicates that no order by clause has been entered and we will not sort
        /// the query, empty sorts will cause the default sorts to be used.
        /// </summary>
        /// <returns></returns>
        IOrderBy OrderByClause();

        /// <summary>
        /// Returns the original query string that the user inputted into the system. 
        /// If not provided, will return null.
        /// </summary>
        /// <returns></returns>
        string QueryString();
    }
}