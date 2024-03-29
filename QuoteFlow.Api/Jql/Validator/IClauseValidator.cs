﻿using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Api.Jql.Validator
{
    /// <summary>
    /// Validates a clause and adds human readable i18n'ed messages if there is a problem.
    /// </summary>
    public interface IClauseValidator
    {
        /// <summary>
        /// Validates a clause and adds human readable i18n'ed messages if there is a problem.
        /// </summary>
        /// <param name="searcher">The user who is executing the search.</param>
        /// <param name="terminalClause">The clause to validate.</param>
        /// <returns>
        /// A MessageSet that will contain any messages relating to failed validation. An empty message set must
        /// be returned to indicate there were no errors. null can never be returned.
        /// </returns>
        IMessageSet Validate(User searcher, ITerminalClause terminalClause);
    }
}