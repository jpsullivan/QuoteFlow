using QuoteFlow.Models.Search.Jql.Query.Clause;

namespace QuoteFlow.Models.Search.Jql.Context
{
    /// <summary>
    /// Creates a <see cref="IClauseContext"/> for the associated clause.
    /// </summary>
    public interface IClauseContextFactory
    {
        /// <summary>
        /// Generates a clause context for the associated handler. If the clause context could not be determined for any
        /// reason, this will return the Global Clause Context.
        /// </summary>
        /// <param name="searcher"> the user who is performing the search </param>
        /// <param name="terminalClause"> the clause for which this factory is generating a context. </param>
        /// <returns>
        /// ClauseContext that contains the implied and explicit project and issue types that this
        /// clause is in context for.
        /// </returns>
        IClauseContext GetClauseContext(User searcher, ITerminalClause terminalClause);
    }
}