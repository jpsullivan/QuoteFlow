﻿using Lucene.Net.QueryParsers;
using QuoteFlow.Infrastructure.Lucene;
using QuoteFlow.Models.Assets.Index;

namespace QuoteFlow.Models.Search.Jql.Query.Lucene.Parsing
{
    public class LuceneQueryParserFactory : ILuceneQueryParserFactory
    {
        public QueryParser CreateParserFor(string fieldName)
        {
            return new LuceneQueryParser(LuceneVersion.Get(), fieldName, QuoteFlowAnalyzer.ANALYZER_FOR_SEARCHING);
        }
    }
}