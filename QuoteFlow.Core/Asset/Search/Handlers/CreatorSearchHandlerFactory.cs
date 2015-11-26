using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Asset.Fields;
using QuoteFlow.Core.Asset.Search.Searchers;
using QuoteFlow.Core.Jql.Context;
using QuoteFlow.Core.Jql.Permission;
using QuoteFlow.Core.Jql.Query;
using QuoteFlow.Core.Jql.Validator;
using QuoteFlow.Core.Jql.Values;

namespace QuoteFlow.Core.Asset.Search.Handlers
{
    /// <summary>
    /// Class to create the <see cref="SearchHandler"/> for the <see cref="CreatorSystemField"/>
    /// </summary>
    public class CreatorSearchHandlerFactory : SimpleSearchHandlerFactory
    {
        public CreatorSearchHandlerFactory(CreatorClauseQueryFactory clauseQueryFactory, 
            CreatorValidator clauseValidator, FieldClausePermissionChecker.IFactory clausePermissionFactory, 
            IUserService userService)
            : base(SystemSearchConstants.ForCreator(), typeof(CreatorSearcher), 
                  clauseQueryFactory, clauseValidator, clausePermissionFactory,
                  new SimpleClauseContextFactory(),
                  new UserClauseValuesGenerator(userService))
        {
        }
    }
}