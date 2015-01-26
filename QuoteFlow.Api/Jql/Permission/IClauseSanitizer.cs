using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Jql.Permission
{
    /// <summary>
    /// Deals with the sanitisation of clauses based on the given user.
    /// </summary>

    public interface IClauseSanitizer
    {
        /// <summary>
        /// Given a user and a clause, will return a sanitised clause that when possible will not contain any information
        /// that the specified user does not have permission to see. For example, if the given clause names a project that
        /// the user cannot browse, a sanitiser might return a new clause with the name of the project replaced with the id.
        /// 
        /// It is important that the returned clause is equivalent to the input clause, within the constraints of the
        /// permissible clauses for the specified user.
        /// </summary>
        /// <param name="user">The user performing the search.</param>
        /// <param name="clause">The clause to be sanitised.</param>
        /// <returns>The sanitised clause; never null.</returns>
        IClause Sanitize(User user, ITerminalClause clause);
    }
}