namespace QuoteFlow.Models.Search.Jql.Permission
{
    /// <summary>
    /// Checks to see that the provided user is able to use the clause.
    /// </summary>
    public interface IClausePermissionChecker
    {
        /// <summary>
        /// Checks to see that the provided user is able to use the clause. This may be as simple as
        /// determining if the user has permission to see the field that the clause represents.
        /// </summary>
        /// <param name="user">The user to check permissions against.</param>
        /// <returns>True if the user can use this clause, otherwise false.</returns>
        bool HasPermissionToUseClause(User user);
    }
}