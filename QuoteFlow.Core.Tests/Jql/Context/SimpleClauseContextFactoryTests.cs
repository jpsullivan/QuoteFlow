using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Core.Jql.Context;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Context
{
    public class SimpleClauseContextFactoryTests
    {
        [Fact]
        public void TestGetClauseContext()
        {
            var factory = new SimpleClauseContextFactory();
            var expectedResult = ClauseContext.CreateGlobalClauseContext();
            var result = factory.GetClauseContext(null, new TerminalClause("blah", Operator.EQUALS, "blah"));
            Assert.Equal(expectedResult, result);
        }
    }
}