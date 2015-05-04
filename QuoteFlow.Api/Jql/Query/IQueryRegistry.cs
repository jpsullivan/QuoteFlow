using System.Collections.Generic;
using QuoteFlow.Api.Jql.Query.Clause;

namespace QuoteFlow.Api.Jql.Query
{
    /// <summary>
    /// Used to map a <see cref="ITerminalClause"/> to its associated<see cref="IClauseQueryFactory"/>s.
    /// </summary>
    public interface IQueryRegistry
    {
        /// <summary>
        /// Fetches all associated ClauseQueryFactory objects for the provided TerminalClause. The returned value is based on
        /// the clauses name the <see cref="Operator"/> that is associated with the provided clause. 
        /// Multiple values may be returned for custom fields.
        /// </summary>
        /// <param name="queryCreationContext">The context for creating the query.</param>
        /// <param name="clause">The clause that defines the name and operator for which we want to find the query factories, must not be null.</param>
        /// <returns>The query factories associated with this clause. The empty list will be returned to indicate failure.</returns>
        ICollection<IClauseQueryFactory> GetClauseQueryFactory(IQueryCreationContext queryCreationContext, ITerminalClause clause);
    }
}