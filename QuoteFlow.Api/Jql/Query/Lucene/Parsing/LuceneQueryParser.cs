using Lucene.Net.Analysis;
using Lucene.Net.QueryParsers;
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
            // todo: add `quoted` section analyzer for exact searching
            return base.GetFieldQuery(field, queryText);
        }
    }
}