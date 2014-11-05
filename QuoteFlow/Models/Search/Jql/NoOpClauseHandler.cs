using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuoteFlow.Infrastructure;
using QuoteFlow.Models.Assets.Search;
using QuoteFlow.Models.Assets.Search.Constants;
using QuoteFlow.Models.Search.Jql.Clauses;
using QuoteFlow.Models.Search.Jql.Context;
using QuoteFlow.Models.Search.Jql.Permission;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Validator;

namespace QuoteFlow.Models.Search.Jql
{
    /// <summary>
    /// A clause handler that does nothing and will show the passed in error message when validate is invoked. This will
    /// return a false query if the query factory is invoked, it will also generate an All context if asked for its
    /// context. The permission handler is the one that is passed in.
    /// </summary>
    public class NoOpClauseHandler : IClauseHandler
    {
        private readonly IClauseInformation clauseInformation;
        private readonly IClauseQueryFactory clauseQueryFactory;
        private readonly IClauseValidator clauseValidator;
        private readonly ClausePermissionHandler clausePermissionHandler;
        private readonly IClauseContextFactory clauseContextFactory;

        public NoOpClauseHandler(ClausePermissionHandler clausePermissionHandler, string fieldId, ClauseNames clauseNames, string validationI18nKey)
        {
            this.clausePermissionHandler = clausePermissionHandler;
            this.clauseInformation = new ClauseInformation(fieldId, clauseNames, fieldId, Enumerable.Empty<Operator>(), QuoteFlowDataTypes.All);
            this.clauseQueryFactory = new ClauseQueryFactoryAnonymousInnerClassHelper(this);
            this.clauseValidator = new ClauseValidatorAnonymousInnerClassHelper(this, validationI18nKey);
            this.clauseContextFactory = new SimpleClauseContextFactory();
        }

        public IClauseInformation Information { get; private set; }
        public IClauseQueryFactory Factory { get; private set; }
        public IClauseValidator Validator { get; private set; }
        public IClauseContextFactory ClauseContextFactory { get; private set; }
    }
}