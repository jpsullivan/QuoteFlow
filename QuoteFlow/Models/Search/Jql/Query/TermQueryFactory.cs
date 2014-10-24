using Lucene.Net.Index;
using Lucene.Net.Search;
using QuoteFlow.Models.Assets.Index;

namespace QuoteFlow.Models.Search.Jql.Query
{
    /// <summary>
    /// </summary>
    internal sealed class TermQueryFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName">The index field to be visible.</param>
        /// <returns>The term query <code>visiblefieldids:fieldName</code></returns>
        internal static global::Lucene.Net.Search.Query VisibilityQuery(string fieldName)
        {
            return new TermQuery(new Term(DocumentConstants.AssetVisibleFieldIds, fieldName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName">The index field to be non empty</param>
        /// <returns>The term query <code>nonemptyfieldids:fieldName</code></returns>
        internal static global::Lucene.Net.Search.Query NonEmptyQuery(string fieldName)
        {
            return new TermQuery(new Term(DocumentConstants.AssetNonEmptyFieldIds, fieldName));
        }
    }
}