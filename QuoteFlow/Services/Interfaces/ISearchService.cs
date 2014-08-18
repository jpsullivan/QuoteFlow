using System.Threading.Tasks;
using QuoteFlow.Models.Search;

namespace QuoteFlow.Services.Interfaces
{
    public interface ISearchService
    {
        /// <summary>
        /// Searches for assets that match the search filter and returns a set of results.
        /// </summary>
        /// <param name="filter">The search filter to be used.</param>
        /// <returns>The number of hits in the search and, if the CountOnly flag in SearchFilter was false, the results themselves</returns>
        Task<SearchResult> Search(SearchFilter filter);
    }
}