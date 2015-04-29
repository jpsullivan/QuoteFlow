using Lucene.Net.Analysis;
using Lucene.Net.QueryParsers;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Asset.Index.Indexers.Phrase;
using Version = Lucene.Net.Util.Version;

namespace QuoteFlow.Api.Jql.Query.Lucene.Parsing
{
    public class LuceneQueryParser : QueryParser
    {
        public LuceneQueryParser(Version matchVersion, string f, Analyzer a) : base(matchVersion, f, a)
        {
        }

        protected internal LuceneQueryParser(ICharStream stream) : base(stream)
        {
        }

        protected LuceneQueryParser(QueryParserTokenManager tm) : base(tm)
        {
        }

        protected override global::Lucene.Net.Search.Query GetFieldQuery(string field, string queryText)
        {
            // todo: lucene 4.8
//            if (quoted)
//            {
//                return NewFieldQuery(QuoteFlowAnalyzer.AnalyzerForExactSearching, PhraseQuerySupportField.ForIndexField(field), queryText, quoted);
//            }
//
//            return base.GetFieldQuery(field, queryText, quoted);

            // todo: add `quoted` section analyzer for exact searching
            return base.GetFieldQuery(field, queryText);
        }
    }
}