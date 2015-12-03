using System.Collections.Generic;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Asset.Search.Searchers.Util
{
    /// <summary>
    /// Interface for clauses that parse JQL clauses and determine if they are suitable for usage
    /// in the Navigator or Search URL.
    /// </summary>
    public interface ICostSearcherInputHelper
    {
        /// <summary>
        /// Take the passed clause and try to get the equivalent navigator params.
        /// 
        /// Note: this also performs a validity check on the structure to determine
        /// if it fits for the navigator. Therefore, it is not required to check
        /// this before the call is made.
        /// </summary>
        /// <param name="clause">The clause to convert</param>
        /// <param name="user">The user trying to convert the clause</param>
        /// <returns>On success a map of navigator param -> value, or null on failure.</returns>
        IDictionary<string, string> ConvertClause(IClause clause, User user);
    }
}