using QuoteFlow.Models.Assets.Search.Constants;
using QuoteFlow.Models.Assets.Search.Searchers;
using QuoteFlow.Models.Search.Jql.Context;
using QuoteFlow.Models.Search.Jql.Permission;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Validator;

namespace QuoteFlow.Models.Assets.Search.Handlers
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