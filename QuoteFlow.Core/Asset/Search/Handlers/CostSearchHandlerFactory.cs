using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Jql.Permission;
using QuoteFlow.Core.Asset.Search.Searchers;
using QuoteFlow.Core.Jql.Context;
using QuoteFlow.Core.Jql.Permission;
using QuoteFlow.Core.Jql.Query;
using QuoteFlow.Core.Jql.Validator;

namespace QuoteFlow.Core.Asset.Search.Handlers
{
    /// <summary>
    /// Class to reate the search handler for the cost ratio clause.
    /// </summary>
    public class CostSearchHandlerFactory : SimpleSearchHandlerFactory
    {
        public CostSearchHandlerFactory(CostClauseQueryFactory costClauseQueryFactory, CostValidator costValidator, FieldClausePermissionChecker.IFactory clausePermissionFactory)
            : base(SystemSearchConstants.ForCost(), typeof(CostSearcher), costClauseQueryFactory, costValidator, new CostClausePermissionCheckerFactory(clausePermissionFactory),
                new SimpleClauseContextFactory(), null)
        {
        }

        private class CostClausePermissionCheckerFactory : FieldClausePermissionChecker.IFactory
        {
            private readonly FieldClausePermissionChecker.IFactory _clausePermissionFactory;

            public CostClausePermissionCheckerFactory(FieldClausePermissionChecker.IFactory clausePermissionFactory)
            {
                _clausePermissionFactory = clausePermissionFactory;
            }

            public IClausePermissionChecker CreatePermissionChecker(IField field)
            {
                return null;
            }

            public IClausePermissionChecker CreatePermissionChecker(string fieldId)
            {
                return null;
            }
        }
    }
}