using QuoteFlow.Models.Assets.Fields;

namespace QuoteFlow.Models.Search.Jql.Permission
{
    /// <summary>
    /// A clause permission checker that will check only the users permission to see the field,
    /// based on the configured field configuration schemes.
    /// </summary>
    public class FieldClausePermissionChecker : IClausePermissionChecker
    {
        public bool HasPermissionToUseClause(User user)
        {
            return true;
        }
    }

    public interface IFactory
    {
        IClausePermissionChecker CreatePermissionChecker(IField field);

        IClausePermissionChecker CreatePermissionChecker(string fieldId);
    }
}