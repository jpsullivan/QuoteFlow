using Moq;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Core.Jql.Builder;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Builder
{
    public class NotMutableClauseTests
    {
        [Fact]
        public void TestCombine()
        {
            NotMutableClause clause1 = new NotMutableClause(new SingleMutableClause(new TerminalClause("one", Operator.GREATER_THAN, "one")));
            NotMutableClause clause2 = new NotMutableClause(new SingleMutableClause(new TerminalClause("two", Operator.GREATER_THAN, "two")));
            NotMutableClause clause3 = new NotMutableClause(new SingleMutableClause(new TerminalClause("three", Operator.GREATER_THAN, "three")));

            Assert.Equal(clause1.Combine(BuilderOperator.OR, clause2), new MultiMutableClause<IMutableClause>(BuilderOperator.OR, clause1, clause2));
            Assert.Equal(clause1.Combine(BuilderOperator.AND, clause3), new MultiMutableClause<IMutableClause>(BuilderOperator.AND, clause1, clause3));
            Assert.Equal(clause1.Combine(BuilderOperator.NOT, null), new NotMutableClause(clause1));
        }

        [Fact]
        public void TestAsClause()
        {
            TerminalClause clause = new TerminalClause("one", Operator.GREATER_THAN, "one");
            NotMutableClause mutableClause = new NotMutableClause(new SingleMutableClause(clause));
            Assert.Equal(new NotClause(clause), mutableClause.AsClause());

            mutableClause = new NotMutableClause(new MockMutableClause(null));
            Assert.Null(mutableClause.AsClause());
        }

        [Fact]
        public void TestCopyNoCopy()
        {
            var mockClause = new Mock<IMutableClause>();
            mockClause.Setup(x => x.Copy()).Returns(new MockMutableClause(null));

            NotMutableClause notMutableClause = new NotMutableClause(mockClause.Object);
            Assert.Same(notMutableClause, notMutableClause.Copy());

            mockClause.Verify();
        }

        [Fact]
        public void TestCopyRealCopy()
        {
            var mockClause = new Mock<IMutableClause>();
            mockClause.Setup(x => x.Copy()).Returns(new MockMutableClause(null));

            NotMutableClause notMutableClause = new NotMutableClause(mockClause.Object);
            IMutableClause actualCopy = notMutableClause.Copy();

            Assert.NotSame(notMutableClause, actualCopy);
            Assert.Equal(new NotMutableClause(new MockMutableClause(null)), actualCopy);

           mockClause.Verify();
        }

        [Fact]
        public void TestToString()
        {
            IMutableClause clause = new NotMutableClause(new MockMutableClause(null, "qwerty"));
            Assert.Equal("NOT(qwerty)", clause.ToString());
        }
    }
}