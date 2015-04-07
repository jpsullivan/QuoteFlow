using System;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Core.Jql.Builder;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Builder
{
    public class PrecedenceSimpleClauseBuilderTests
    {
        [Fact]
        public void TestStartState()
        {
            var builder = new PrecedenceSimpleClauseBuilder();
            Assert.Null(builder.Build());
            Assert.Equal(String.Empty, builder.ToString());
            Assert.Equal(String.Empty, builder.Copy().ToString());

            builder.And();

            Assert.Null(builder.Build());
            Assert.Equal(String.Empty, builder.ToString());
            Assert.Equal(String.Empty, builder.Copy().ToString());

            builder.Or();

            Assert.Null(builder.Build());
            Assert.Equal(String.Empty, builder.ToString());
            Assert.Equal(String.Empty, builder.Copy().ToString());

            try
            {
                builder.Endsub();
                Assert.True(false, "Expected exception.");
            }
            catch (Exception ex)
            {
                // expected
            }

            var expectedClause = new TerminalClause("test", Operator.EQUALS, "pass");
            ISimpleClauseBuilder copy = builder.Copy();
            Assert.Equal(expectedClause, copy.Clause(expectedClause).Build());

            copy = builder.Copy();
            Assert.Null(copy.Build());
        }
    }
}