using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Jql.Permission
{
    /// <summary>
    /// The default implementation of a <see cref="IClausePermissionHandler"/>. To fulfill the responsibilities
    /// of the composite interfaces, this class simply holds one reference to each interface, and delegates to 
    /// those instances.
    /// </summary>
    public class ClausePermissionHandler : IClausePermissionHandler
    {
        public static readonly ClausePermissionHandler NoopClausePermissionHandler = new ClausePermissionHandler(NoOpClausePermissionChecker.NoopClausePermissionChecker);

        private readonly IClausePermissionChecker permissionChecker;
        private readonly IClauseSanitizer sanitizer;

        public ClausePermissionHandler(IClausePermissionChecker permissionChecker) : this(permissionChecker, NoOpClauseSanitizer.NoopClauseSanitizer)
		{
		}

		public ClausePermissionHandler(IClauseSanitizer sanitiser) : this(NoOpClausePermissionChecker.NoopClausePermissionChecker, sanitiser)
		{
		}

        public ClausePermissionHandler(IClausePermissionChecker permissionChecker, IClauseSanitizer sanitizer)
		{
			this.permissionChecker = permissionChecker;
			this.sanitizer = sanitizer;
		}

        public IClause Sanitize(User user, ITerminalClause clause)
        {
            return sanitizer.Sanitize(user, clause);
        }

        public bool HasPermissionToUseClause(User user)
        {
            return permissionChecker.HasPermissionToUseClause(user);
        }

        protected bool Equals(ClausePermissionHandler other)
        {
            return Equals(permissionChecker, other.permissionChecker) && Equals(sanitizer, other.sanitizer);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ClausePermissionHandler) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((permissionChecker != null ? permissionChecker.GetHashCode() : 0)*397) ^ (sanitizer != null ? sanitizer.GetHashCode() : 0);
            }
        }
    }
}