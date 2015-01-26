using Lucene.Net.QueryParsers;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Jql.Query.Lucene.Parsing;
using QuoteFlow.Api.Lucene.Index;

namespace QuoteFlow.Core.Jql.Query.Lucene.Parsing
{
    public class LuceneQueryParserFactory : ILuceneQueryParserFactory
    {
        public QueryParser CreateParserFor(string fieldName)
        {
            return new LuceneQueryParser(LuceneVersion.Get(), fieldName, QuoteFlowAnalyzer.AnalyzerForSearching);
        }
    }
}