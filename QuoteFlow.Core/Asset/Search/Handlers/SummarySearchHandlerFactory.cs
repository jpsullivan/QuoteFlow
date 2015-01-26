using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Search.Searchers;
using QuoteFlow.Api.Jql.Permission;
using QuoteFlow.Core.Asset.Search.Searchers;
using QuoteFlow.Core.Jql.Context;
using QuoteFlow.Core.Jql.Permission;
using QuoteFlow.Core.Jql.Query;
using QuoteFlow.Core.Jql.Validator;

namespace QuoteFlow.Core.Asset.Search.Handlers
{
    /// <summary>
    /// Class to create the <see cref="SearchHandler"/> for the <see cref="SummarySearchHandlerFactory"/>.
    /// </summary>
    public sealed class SummarySearchHandlerFactory : SimpleSearchHandlerFactory
    {
        public SummarySearchHandlerFactory(SummaryClauseQueryFactory queryFactory, 
            SummaryValidator queryValidator, FieldClausePermissionChecker.IFactory clausePermissionFactory)
            : base(SystemSearchConstants.ForSummary(), typeof(SummaryQuerySearcher), queryFactory, queryValidator, clausePermissionFactory, new SimpleClauseContextFactory(), null)
        {
        }
    }
}