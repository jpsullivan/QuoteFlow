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

        [Fact]
        public void TestNotState()
        {
            var builder = new PrecedenceSimpleClauseBuilder();
            builder.Not();
            Assert.Equal("NOT", builder.ToString());

            try
            {
                builder.And();
                Assert.True(false, "Expected exception");
            }
            catch (Exception ex)
            {
                // expected
            }

            try
            {
                builder.Or();
                Assert.True(false, "Expected exception");
            }
            catch (Exception ex)
            {
                // expected
            }

            try
            {
                builder.Build();
                Assert.True(false, "Expected exception");
            }
            catch (Exception ex)
            {
                // expected
            }

            try
            {
                builder.Endsub();
                Assert.True(false, "Expected exception");
            }
            catch (Exception ex)
            {
                // expected
            }

            builder.Not();
            Assert.Equal("NOT NOT", builder.ToString());

            try
            {
                builder.And();
                Assert.True(false, "Expected exception");
            }
            catch (Exception ex)
            {
                // expected
            }

            try
            {
                builder.Or();
                Assert.True(false, "Expected exception");
            }
            catch (Exception ex)
            {
                // expected
            }

            try
            {
                builder.Build();
                Assert.True(false, "Expected exception");
            }
            catch (Exception ex)
            {
                // expected
            }

            try
            {
                builder.Endsub();
                Assert.True(false, "Expected exception");
            }
            catch (Exception ex)
            {
                // expected
            }

            Assert.Equal("NOT NOT NOT", builder.Copy().Not().ToString());

            var expectedClause = new TerminalClause("test", Operator.EQUALS, "pass");
            var expectedClause2 = new TerminalClause("test2", Operator.EQUALS, "pass2");
            var copy = builder.Copy();

            copy.Clause(expectedClause);
            Assert.Equal(new NotClause(new NotClause(expectedClause)), copy.Build());

            builder.Sub().Clause(expectedClause).Or().Clause(expectedClause2).Endsub();
            Assert.Equal(new NotClause(new NotClause(new OrClause(expectedClause, expectedClause2))), builder.Build());
        }
    }
}