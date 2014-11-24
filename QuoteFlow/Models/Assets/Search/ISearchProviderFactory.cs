using Lucene.Net.Search;

namespace QuoteFlow.Models.Assets.Search
{
    /// <summary>
    /// Provides low-level searches that can be used to query indexes.  In JIRA's case these are
    /// Lucene <seealso cref="IndexSearcher"/>s.
    /// <p/>
    /// <b>NOTE:</b> This class should perhaps be re-name to 'SearcherFactory' as <seealso cref="SearchProvider"/>s are something
    /// a little different in JIRA.
    /// </summary>
    public interface ISearchProviderFactory
    {

        /// <summary>
        /// Get a Lucene <seealso cref="IndexSearcher"/> that can be used to search a Lucene index.
        /// <p/>
        /// At the moment the possible values for the searcherName argument are <seealso cref="#ISSUE_INDEX"/> (to search the
        /// issue index) and <seealso cref="#COMMENT_INDEX"/> (to search the comment index).
        /// </summary>
        IndexSearcher GetSearcher(string searcherName);
    }

    public static class SearchProviderTypes
    {
        public const string ISSUE_INDEX = "assets";
        public const string COMMENT_INDEX = "comments";
        public const string CHANGE_HISTORY_INDEX = "changes";
    }
}