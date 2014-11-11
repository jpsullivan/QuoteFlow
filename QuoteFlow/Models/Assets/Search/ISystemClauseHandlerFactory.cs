using System.Collections.Generic;

namespace QuoteFlow.Models.Assets.Search
{
    /// <summary>
    /// Provides access to System clause handlers, through <see cref="SearchHandler.SearcherRegistration"/>'s
    /// for clauses that do not have associated system fields and searchers.
    /// </summary>
    public interface ISystemClauseHandlerFactory
    {
        /// <summary>
        /// Will return a collection of SearchHandlers that represent the system clause search handlers that are not
        /// associated with a field or a searcher.
        /// </summary>
        /// <returns> SearchHandlers that represent the system clause search handlers that are not
        /// associated with a field or a searcher. </returns>
        IEnumerable<SearchHandler> GetSystemClauseSearchHandlers();
    }
}