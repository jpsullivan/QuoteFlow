using QuoteFlow.Models.Search.Jql.Context;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Validator;

namespace QuoteFlow.Models.Search.Jql.Clauses
{
    public interface IClauseHandler
    {
        /// <summary>
        /// An object that contains some static naming information (clause names, field id, index id) 
        /// about the clause handler and its associations. 
        /// </summary>
        IClauseInformation Information { get; }

        /// <summary>
        /// A factory that can create a lucene query for the clause. 
        /// </summary>
        IClauseQueryFactory Factory { get; }

        /// <summary>
        /// A validator that will inspect the clause and return any validation errors it encounters.
        /// </summary>
        IClauseValidator Validator { get; }

        /// <summary>
        /// A permission handler that will check the users who is executing the queries 
        /// permission to include the clause.
        /// </summary>
        //ClausePermissionHandler PermissionHandler { get; }

        /// <summary>
        /// A clause context factory that will be able to generate the clause context.
        /// </summary>
        IClauseContextFactory ClauseContextFactory { get; }
    }

}