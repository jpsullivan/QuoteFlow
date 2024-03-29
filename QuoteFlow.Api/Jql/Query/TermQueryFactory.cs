﻿using Lucene.Net.Index;
using Lucene.Net.Search;
using QuoteFlow.Api.Asset.Index;

namespace QuoteFlow.Api.Jql.Query
{
    /// <summary>
    /// </summary>
    public sealed class TermQueryFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName">The index field to be visible.</param>
        /// <returns>The term query <code>visiblefieldids:fieldName</code></returns>
        public static global::Lucene.Net.Search.Query VisibilityQuery(string fieldName)
        {
            return new TermQuery(new Term(DocumentConstants.AssetVisibleFieldIds, fieldName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName">The index field to be non empty</param>
        /// <returns>The term query <code>nonemptyfieldids:fieldName</code></returns>
        public static global::Lucene.Net.Search.Query NonEmptyQuery(string fieldName)
        {
            return new TermQuery(new Term(DocumentConstants.AssetNonEmptyFieldIds, fieldName));
        }
    }
}