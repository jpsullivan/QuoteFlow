using QuoteFlow.Models.Search.Jql.Query.Clause;

namespace QuoteFlow.Models.Search.Jql.Context
{
    public class SimpleClauseContextFactory : IClauseContextFactory
    {
        public IClauseContext GetClauseContext(User searcher, ITerminalClause terminalClause)
        {
            return ClauseContext.CreateGlobalClauseContext();
        }
    }
}