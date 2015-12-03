using System;
using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Permission;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Validator;
using QuoteFlow.Api.Jql.Values;
using QuoteFlow.Core.Jql.Permission;

namespace QuoteFlow.Core.Asset.Search.Handlers
{
    /// <summary>
    /// Class to reate the search handler for the cost ratio clause.
    /// </summary>
    public class CostSearchHandlerFactory : SimpleSearchHandlerFactory
    {
        public CostSearchHandlerFactory(IClauseInformation information, Type searcherClass, IClauseQueryFactory queryFactory, IClauseValidator clauseValidator, FieldClausePermissionChecker.IFactory clausePermissionFactory, IClauseContextFactory clauseContextFactory, IClauseValuesGenerator clauseValuesGenerator, IClauseSanitizer sanitizer = null) : base(information, searcherClass, queryFactory, clauseValidator, clausePermissionFactory, clauseContextFactory, clauseValuesGenerator, sanitizer)
        {
        }
    }
}