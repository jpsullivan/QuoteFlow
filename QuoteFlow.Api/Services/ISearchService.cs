using System.Threading.Tasks;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Parser;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Search;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Api.Services
{
    /// <summary>
    /// Provides functionality (search, query string generation, parsing, validation, context generation, etc...) related
    /// to searching in QuoteFlow.
    /// </summary>
    public interface ISearchService
    {
        /// <summary>
        /// Searches for assets that match the search filter and returns a set of results.
        /// </summary>
        /// <param name="filter">The search filter to be used.</param>
        /// <returns>The number of hits in the search and, if the CountOnly flag in SearchFilter was false, the results themselves</returns>
        Task<SearchResult> Search(SearchFilter filter);

        /// <summary>
        /// Search the index, and only return assets that are in the pagers' range.
        /// NOTE: This method returns read-only <see cref="Asset"/> objects, and should not be used
        /// where you need the Asset for update.
        /// 
        /// Also note that if you are running a more complicated query, see <seealso cref="SearchProvider"/>.
        /// </summary>
        /// <param name="searcher">The user performing the search, which will be used to create a permission 
        /// filter that filters out any of the results the user is not able to see and will be used to provide 
        /// context for the search.</param>
        /// <param name="query">Contains the information required to perform the search.</param>
        /// <returns></returns>
        SearchResults Search(User searcher, IQuery query);

        /// <summary>
        /// Search the index and return the count of the assets matching the query.
        /// </summary>
        /// <param name="searcher">The user performing the search.</param>
        /// <param name="query">Contains the information required to perform the search.</param>
        /// <returns></returns>
        long SearchCount(User searcher, IQuery query);

        /// <summary>
        /// Parses the query string into a JQL <see cref="Query"/>.
        /// </summary>
        /// <param name="searcher">The user in context.</param>
        /// <param name="query">The raw query string to parse into a <see cref="Query"/> object.</param>
        /// <returns></returns>
        string GetQueryString(User searcher, string query);

        /// <summary>
        /// Parses the query into a JQL <see cref="Query"/>.
        /// </summary>
        /// <param name="searcher">The user performing the search.</param>
        /// <param name="query">The query to parse into a <see cref="Query"/>.</param>
        /// <returns>A result set that contains the query and a message set of any errors/warnings.</returns>
        ParseResult ParseQuery(User searcher, string query);

        /// <summary>
        /// Validates the specified <see cref="IQuery"/> for passed user. The same as calling
        /// ValidateQuery(searcher, query, null);
        /// </summary>
        /// <param name="searcher">The user performing the search.</param>
        /// <param name="query">The search query to validate.</param>
        /// <returns></returns>
        IMessageSet ValidateQuery(User searcher, IQuery query);

        /// <summary>
        /// Validates the specified <see cref="IQuery"/> for passed user and search request. 
        /// This validates the the passed query as if it was run as the passed search request.
        /// </summary>
        /// <param name="searcher">The user performing the search.</param>
        /// <param name="query">The search query to validate.</param>
        /// <param name="searchRequestId">Validate in the context of this search request.</param>
        /// <returns></returns>
        IMessageSet ValidateQuery(User searcher, IQuery query, long searchRequestId);

        /// <summary>
        /// Generates a full QueryContext for the specified <see cref="IQuery"/> for the searching user. 
        /// The full QueryContext contains all explicit and implicitly specified catalogs and manufacturers 
        /// from the Query.
        /// For a better explanation of the differences between the full and simple QueryContexts, see
        /// <see cref="QueryContextVisitor"/>.
        /// </summary>
        /// <param name="searcher">The user performing the search.</param>
        /// <param name="query">The search query to generate the context for.</param>
        /// <returns>A QueryContext that contains the implicit and explicit catalog / manufacturers implied by the included clauses in the query.</returns>
        IQueryContext GetQueryContext(User searcher, IQuery query);

        /// <summary>
        /// Generates a simple QueryContext for the specified <seealso cref="IQuery"/> for the searching user.
        /// The simple QueryContext contains only the explicit projects and issue types specified in the Query. If none were
        /// specified, it will be the Global context.
        /// For a better explanation of the differences between the full and simple QueryContexts, see
        /// <seealso cref="QueryContextVisitor"/>.
        /// </summary>
        /// <param name="searcher">The user performing the search</param>
        /// <param name="query">The search query to generate the context for.</param>
        /// <returns>A QueryContext that contains only the explicit catalog / manufacturers from the included clauses in the query.</returns>
        IQueryContext GetSimpleQueryContext(User searcher, IQuery query);

        /// <summary>
        /// This produces an old-style <see cref="SearchContext"/> based on the passed in
        /// search query and the user that is performing the search.
        /// 
        /// This will only make sense if the query returns true for <seealso cref="#doesQueryFitFilterForm(com.atlassian.crowd.embedded.api.User, com.atlassian.query.Query)"/>
        /// since SearchContext is only relevant for simple queries.
        /// 
        /// The more acurate context can be gotten by calling <seealso cref="#getQueryContext(com.atlassian.crowd.embedded.api.User, com.atlassian.query.Query)"/>.
        /// 
        /// If the query will not fit in the simple issue navigator then the generated SearchContext will be empty. This
        /// method never returns a null SearchContext, even when passed a null SearchQuery.
        /// </summary>
        /// <param name="searcher"> the user performing the search, not always the SearchRequest's owner </param>
        /// <param name="query"> the query for which you want a context </param>
        /// <returns> a SearchContext with the correct project/issue types if the query fits in the issue navigator, otherwise
        /// an empty SearchContext. Never null. </returns>
        ISearchContext GetSearchContext(User searcher, IQuery query);

        /// <summary>
        /// Gets the JQL string representation for the passed query. Returns the string from <seealso cref="com.atlassian.query.Query#getQueryString()"/>
        /// if it exists or generates one if it does not. Equilavent to:
        /// <pre>
        ///  if (query.getQueryString() != null)
        ///    return query.getQueryString();
        ///  else
        ///    return getGeneratedJqlString(query);
        /// 
        /// </pre>
        /// </summary>
        /// <param name="query"> the query. Cannot be null. </param>
        /// <returns> the JQL string represenation of the passed query. </returns>
        string GetJqlString(IQuery query);

        /// <summary>
        /// Generates a JQL string representation for the passed query. The JQL string is always generated, that is, <seealso cref="com.atlassian.query.Query#getQueryString()"/>
        /// is completely ignored if it exists. The returned JQL is automatically escaped as necessary.
        /// </summary>
        /// <param name="query"> the query. Cannot be null. </param>
        /// <returns> the generated JQL string representation of the passed query. </returns>
        string GetGeneratedJqlString(IQuery query);

    }
}