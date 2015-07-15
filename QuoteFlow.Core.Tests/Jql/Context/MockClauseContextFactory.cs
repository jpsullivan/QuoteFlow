using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Tests.Jql.Context
{
    public class MockClauseContextFactory : IClauseContextFactory
    {
        private readonly MockClauseContext _clauseContext;

        public MockClauseContextFactory()
        {
            _clauseContext = new MockClauseContext();
        }

        public IClauseContext GetClauseContext(User searcher, ITerminalClause terminalClause)
        {
            return _clauseContext;
        }
    }
}