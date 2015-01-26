using Lucene.Net.Search;

namespace QuoteFlow.Api.Asset.Search
{
    /// <summary>
    /// Provides low-level searches that can be used to query indexes.  In QuoteFlow's case these are
    /// Lucene <seealso cref="IndexSearcher"/>s.
    /// 
    /// NOTE: This class should perhaps be re-name to 'SearcherFactory' as <see cref="ISearchProvider"/>s are something
    /// a little different in QuoteFlow.
    /// </summary>
    public interface ISearchProviderFactory
    {
        /// <summary>
        /// Get a Lucene <seealso cref="IndexSearcher"/> that can be used to search a Lucene index.
        /// 
        /// At the moment the possible values for the searcherName argument are <see cref="#ISSUE_INDEX"/> (to search the
        /// asset index) and <see cref="#COMMENT_INDEX"/> (to search the comment index).
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