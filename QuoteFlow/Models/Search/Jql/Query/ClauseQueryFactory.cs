using Lucene.Net.Search;
using QuoteFlow.Models.Search.Jql.Query.Clause;

namespace QuoteFlow.Models.Search.Jql.Query
{
    public class ClauseQueryFactory : IClauseQueryFactory
    {
        public QueryFactoryResult GetQuery(QueryCreationContext queryCreationContext, ITerminalClause terminalClause)
        {
            return new QueryFactoryResult(new BooleanQuery());
        }
    }
}