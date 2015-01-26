using Lucene.Net.Search;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Asset.Search.Parameters.Lucene
{
    public interface IPermissionsFilterGenerator
    {
        /// <summary>
        /// Generates a lucene <seealso cref="Query"/> that is the canonical set of permissions for viewable issues for the given user.
        /// This query can then be used to filter out impermissible documents from a lucene search.
        /// </summary>
        /// <param name="searcher"> the user performing the search </param>
        /// <returns> the query; could be null if an error occurred. </returns>
        Query GetQuery(User searcher);
    }
}