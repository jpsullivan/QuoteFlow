using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Jql.Permission
{
    /// <summary>
    /// A clause permission checker that will check only the users permission to see the field,
    /// based on the configured field configuration schemes.
    /// </summary>
    public class FieldClausePermissionChecker : IClausePermissionChecker
    {
        public IFieldManager FieldManager { get; protected set; }
        public IField Field { get; protected set; }

        public FieldClausePermissionChecker(IFieldManager fieldManager, IField field)
        {
            FieldManager = fieldManager;
            Field = field;
        }

        public bool HasPermissionToUseClause(User user)
        {
            return true;
        }

        public interface IFactory
        {
            IClausePermissionChecker CreatePermissionChecker(IField field);

            IClausePermissionChecker CreatePermissionChecker(string fieldId);
        }

        /// <summary>
        /// This is a factory so that we don't have a circular dependency on the Field manager. It looks like
        /// 
        /// Field Manager -> Field -> SearchHandler -> FieldClausePermissionHandler -> Field Manager.
        /// </summary>
        ///CLOVER:OFF
        public sealed class Factory : IFactory
        {
            public IClausePermissionChecker CreatePermissionChecker(IField field)
            {
                return new FieldClausePermissionChecker(FieldManager, field);
            }

            public IClausePermissionChecker CreatePermissionChecker(string fieldId)
            {
                var field = FieldManager.GetField(fieldId);
                return new FieldClausePermissionChecker(FieldManager, field);
            }

            private static FieldManager FieldManager
            {
                get { return Container.Kernel.TryGet<FieldManager>(); }
            }
        }
    }
}