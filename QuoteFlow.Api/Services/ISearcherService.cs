using System.Collections.Generic;
using QuoteFlow.Api.Infrastructure.Services;
using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Search;
using Wintellect.PowerCollections;

namespace QuoteFlow.Api.Services
{
    /// <summary>
    /// Interface to searchers.
    /// </summary>
    public interface ISearcherService
    {
        IServiceOutcome<QuerySearchResults> Search(User user, MultiDictionary<string, string[]> parameters, long? filterId);

        /// <summary>
        /// Try the given JQL as a basic mode search. If it can't be represented as a basic search, the result
        /// is an error. </summary>
        /// <param name="action"> The Action </param>
        /// <param name="jql"> The JQL requested </param>
        /// <param name="filterId"> The optional Filter ID
        /// </param>
        /// <returns>The result or error from the search.</returns>
        IServiceOutcome<QuerySearchResults> SearchWithJql(User user, string jql, long? filterId);

        IServiceOutcome<SearchRendererValueResults> GetViewHtml(User user, MultiDictionary<string, string[]> parameters);

        IServiceOutcome<string> GetEditHtml(string searcherId, string jqlContext);

        Searchers GetSearchers(User user, string jqlContext);
    }
}