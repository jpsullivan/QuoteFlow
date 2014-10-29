using System.Threading.Tasks;
using QuoteFlow.Infrastructure.Util;
using QuoteFlow.Models;
using QuoteFlow.Models.Assets.Search;
using QuoteFlow.Models.Search;
using QuoteFlow.Models.Search.Jql.Parser;
using QuoteFlow.Models.Search.Jql.Query;

namespace QuoteFlow.Services.Interfaces
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
    }
}