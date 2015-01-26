namespace QuoteFlow.Api.Jql.Permission
{
    /// <summary>
    /// A composite interface that deals with the clause sanitization and permission checking.
    /// An instance of this should be regitered against each clause in the system.
    /// </summary>
    public interface IClausePermissionHandler : IClauseSanitizer, IClausePermissionChecker
    {    
    }
}