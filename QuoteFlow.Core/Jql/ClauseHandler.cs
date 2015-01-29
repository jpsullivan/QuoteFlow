﻿using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Validator;

namespace QuoteFlow.Core.Jql
{
    /// <summary>
    /// A container for all the objects needed to process a Jql clause.
    /// </summary>
    public class ClauseHandler : IClauseHandler
    {
        public IClauseInformation Information { get; private set; }
        public IClauseQueryFactory Factory { get; private set; }
        public IClauseValidator Validator { get; private set; }
        public IClauseContextFactory ClauseContextFactory { get; private set; }

        public ClauseHandler(IClauseInformation information, IClauseQueryFactory factory, IClauseValidator validator, IClauseContextFactory clauseContextFactory)
        {
            Information = information;
            Factory = factory;
            Validator = validator;
            ClauseContextFactory = clauseContextFactory;
        }
    }
}