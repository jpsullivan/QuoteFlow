using System;
using Moq;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Core.Jql.Builder;
using QuoteFlow.Core.Jql.Util;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Builder
{
    public class JqlClauseBuilderTests
    {
        [Fact]
        public void TestClear()
        {
            var mockClauseBuilder = new Mock<ISimpleClauseBuilder>(MockBehavior.Strict);
            var mockClauseBuilder2 = new Mock<ISimpleClauseBuilder>(MockBehavior.Strict);

            mockClauseBuilder.Setup(x => x.Clear()).Returns(mockClauseBuilder2.Object);
            mockClauseBuilder2.Setup(x => x.Clear()).Returns(mockClauseBuilder.Object);

            var builder = CreateBuilder(mockClauseBuilder.Object);
            Assert.Equal(builder, builder.Clear());
            Assert.Equal(builder, builder.Clear());

            mockClauseBuilder.Verify();
            mockClauseBuilder2.Verify();
        }

        [Fact]
        public void TestEmptyCondition()
        {
            var builder = new JqlClauseBuilder();
            try
            {
                builder.AddEmptyCondition(null);
                Assert.True(false, "Expected an exception.");
            }
            catch (ArgumentNullException)
            {
            }

            string clauseName = "name";

            var clauseBuilder = new Mock<ISimpleClauseBuilder>();
            clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.IS, EmptyOperand.Empty)))
                .Returns(clauseBuilder.Object);

            builder = CreateBuilder(clauseBuilder.Object);
            Assert.Equal(builder, builder.AddEmptyCondition(clauseName));

            clauseBuilder.Verify();
        }

        [Fact]
        public void TestEndWhere()
        {
            var queryBuilder = JqlQueryBuilder.NewBuilder();
            var builder = new JqlClauseBuilder(null);
            Assert.Null(builder.EndWhere());

            builder = new JqlClauseBuilder(queryBuilder);
            Assert.Same(queryBuilder, builder.EndWhere());
        }

        [Fact]
        public void TestAddStringRangeConditionString()
        {
            var builder = new JqlClauseBuilder();
            try
            {
                builder.AddStringCondition(null, string.Empty, string.Empty);
                Assert.True(false, "Expected exception.");
            }
            catch (Exception)
            {
            }

            try
            {
                builder.AddStringRangeCondition("qwerty", null, null);
                Assert.True(false, "Expected exception.");
            }
            catch (Exception)
            {
            }

            const string start = "%&*##*$&$";
            const string end = "dhakjdhakjhsdsa";
            const string clauseName = "weqekwq";

            var clauseBuilder = new Mock<ISimpleClauseBuilder>();
            clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.GREATER_THAN_EQUALS, start))).Returns(clauseBuilder.Object);
            clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.LESS_THAN_EQUALS, end))).Returns(clauseBuilder.Object);
            clauseBuilder.Setup(
                x =>
                    x.Clause(new AndClause(new TerminalClause(clauseName, Operator.GREATER_THAN_EQUALS, start),
                        new TerminalClause(
                            clauseName, Operator.LESS_THAN_EQUALS, end)))).Returns(clauseBuilder.Object);

            builder = CreateBuilder(clauseBuilder.Object);
            Assert.Equal(builder, builder.AddStringRangeCondition(clauseName, start, null));
            Assert.Equal(builder, builder.AddStringRangeCondition(clauseName, null, end));
            Assert.Equal(builder, builder.AddStringRangeCondition(clauseName, start, end));
        }

        [Fact]
        public void TestAddNumberRangeConditionString()
        {
            var builder = new JqlClauseBuilder();
            try
            {
                builder.AddNumberRangeCondition(null, 8, 23);
                Assert.True(false, "Expected exception.");
            }
            catch (Exception)
            {
            }

            try
            {
                builder.AddNumberRangeCondition("qwerty", null, null);
                Assert.True(false, "Expected exception.");
            }
            catch (Exception)
            {
            }

            const int start = 538748;
            const int end = 567;
            const string clauseName = "blah";

            var clauseBuilder = new Mock<ISimpleClauseBuilder>();
            clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.GREATER_THAN_EQUALS, start))).Returns(clauseBuilder.Object);
            clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.LESS_THAN_EQUALS, end))).Returns(clauseBuilder.Object);
            clauseBuilder.Setup(
                x =>
                    x.Clause(new AndClause(new TerminalClause(clauseName, Operator.GREATER_THAN_EQUALS, start),
                        new TerminalClause(
                            clauseName, Operator.LESS_THAN_EQUALS, end)))).Returns(clauseBuilder.Object);

            builder = CreateBuilder(clauseBuilder.Object);
            Assert.Equal(builder, builder.AddNumberRangeCondition(clauseName, start, null));
            Assert.Equal(builder, builder.AddNumberRangeCondition(clauseName, null, end));
            Assert.Equal(builder, builder.AddNumberRangeCondition(clauseName, start, end));
        }

        private static JqlClauseBuilder CreateBuilder(ISimpleClauseBuilder clauseBuilder)
        {
            var dateSupport = new JqlDateSupport();
            return CreateBuilder(clauseBuilder, dateSupport);
        }

        private static JqlClauseBuilder CreateBuilder(ISimpleClauseBuilder clauseBuilder, JqlDateSupport dateSupport)
        {
            return new JqlClauseBuilder(null, clauseBuilder, dateSupport);
        }
    }
}