using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lucene.Net.QueryParsers;
using QuoteFlow.Infrastructure.Lucene;

namespace QuoteFlow.Models.Search.Jql.Query.Lucene.Parsing
{
    public class LuceneQueryParserFactory : ILuceneQueryParserFactory
    {
        public QueryParser CreateParserFor(string fieldName)
        {
            return new LuceneQueryParser(LuceneVersion.Get(), fieldName, JiraAnalyzer.ANALYZER_FOR_SEARCHING);
        }
    }
}