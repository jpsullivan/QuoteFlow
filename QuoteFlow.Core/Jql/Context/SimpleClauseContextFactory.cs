using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Jql.Context
{
    public class SimpleClauseContextFactory : IClauseContextFactory
    {
        public IClauseContext GetClauseContext(User searcher, ITerminalClause terminalClause)
        {
            return Api.Jql.Context.ClauseContext.CreateGlobalClauseContext();
        }
    }
}