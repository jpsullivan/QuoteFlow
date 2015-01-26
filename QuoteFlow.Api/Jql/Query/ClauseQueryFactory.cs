using Lucene.Net.Search;
using QuoteFlow.Api.Jql.Query.Clause;

namespace QuoteFlow.Api.Jql.Query
{
    public class ClauseQueryFactory : IClauseQueryFactory
    {
        public QueryFactoryResult GetQuery(IQueryCreationContext queryCreationContext, ITerminalClause terminalClause)
        {
            return new QueryFactoryResult(new BooleanQuery());
        }
    }
}