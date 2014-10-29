namespace QuoteFlow.Models.Search.Jql.Permission
{
    /// <summary>
    /// A no-op permission checker that always allows you to use a clause.
    /// </summary>
    public class NoOpClausePermissionChecker : IClausePermissionChecker
    {
        public static readonly NoOpClausePermissionChecker NoopClausePermissionChecker = new NoOpClausePermissionChecker();

        // shouldn't need construction
        private NoOpClausePermissionChecker() { }

        public bool HasPermissionToUseClause(User user)
        {
            return true;
        }
    }
}