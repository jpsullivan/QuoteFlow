﻿using QuoteFlow.Api;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Permission;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Validator;
using QuoteFlow.Core.Jql.Context;
using QuoteFlow.Core.Jql.Validator;
using Wintellect.PowerCollections;

namespace QuoteFlow.Core.Jql
{
    /// <summary>
    /// A clause handler that does nothing and will show the passed in error message when validate is invoked. This will
    /// return a false query if the query factory is invoked, it will also generate an All context if asked for its
    /// context. The permission handler is the one that is passed in.
    /// </summary>
    public class NoOpClauseHandler : IClauseHandler
    {
        public IClauseInformation Information { get; private set; }
        public IClauseQueryFactory Factory { get; private set; }
        public IClauseValidator Validator { get; private set; }
        public IClauseContextFactory ClauseContextFactory { get; private set; }

        private readonly ClausePermissionHandler clausePermissionHandler;

        public NoOpClauseHandler(ClausePermissionHandler clausePermissionHandler, string fieldId, ClauseNames clauseNames)
        {
            this.clausePermissionHandler = clausePermissionHandler;
            Information = new ClauseInformation(fieldId, clauseNames, fieldId, new Set<Operator>(), QuoteFlowDataTypes.All);
            Factory = new ClauseQueryFactory();
            Validator = new ClauseValidator();
            ClauseContextFactory = new SimpleClauseContextFactory();
        }
    }
}