﻿using Lucene.Net.QueryParsers;

namespace QuoteFlow.Api.Jql.Query.Lucene.Parsing
{
    /// <summary>
    /// A factory to obtain a Lucene <see cref="QueryParser"/> instance.
    /// </summary>
    public interface ILuceneQueryParserFactory
    {
        /// <summary>
        /// Creates a query parser instance.
        /// </summary>
        /// <param name="fieldName">The default field to be used by the query parser for query terms.</param>
        /// <returns>A query parser instance.</returns>
        QueryParser CreateParserFor(string fieldName);
    }
}