using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Permission;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Validator;

namespace QuoteFlow.Api.Jql
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
        IClausePermissionHandler PermissionHandler { get; }

        /// <summary>
        /// A clause context factory that will be able to generate the clause context.
        /// </summary>
        IClauseContextFactory ClauseContextFactory { get; }
    }
}