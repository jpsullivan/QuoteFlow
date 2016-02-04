using QuoteFlow.Api.Jql.Permission;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Tests.Jql.Validator
{
    public class MockClausePermissionChecker : IClausePermissionHandler
    {
        private readonly bool _hasPerm;

        public MockClausePermissionChecker(bool hasPerm)
        {
            _hasPerm = hasPerm;
        }

        public MockClausePermissionChecker() : this(true)
        {
        }

        public IClause Sanitize(User user, ITerminalClause clause)
        {
            return clause;
        }

        public bool HasPermissionToUseClause(User user)
        {
            return _hasPerm;
        }
    }
}