using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Core.Jql.Builder;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Builder
{
    public class SingleMutableClauseTests
    {
        [Fact]
        public void TestCombine()
        {
            var clause1 = new SingleMutableClause(new TerminalClause("one", Operator.GREATER_THAN, "one"));
            var clause2 = new SingleMutableClause(new TerminalClause("two", Operator.GREATER_THAN, "two"));
            var clause3 = new SingleMutableClause(new TerminalClause("three", Operator.GREATER_THAN, "three"));

            Assert.Equal(clause1.Combine(BuilderOperator.OR, clause2), new MultiMutableClause<IMutableClause>(BuilderOperator.OR, clause1, clause2));
            Assert.Equal(clause1.Combine(BuilderOperator.AND, clause3), new MultiMutableClause<IMutableClause>(BuilderOperator.AND, clause1, clause3));
            Assert.Equal(clause1.Combine(BuilderOperator.NOT, null), new NotMutableClause(clause1));
        }

        [Fact]
        public void TestAsClause()
        {
            var clause = new TerminalClause("one", Operator.GREATER_THAN, "one");
            var mutableClause = new SingleMutableClause(clause);
            Assert.Same(clause, mutableClause.AsClause());
        }

        [Fact]
        public void TestCopy()
        {
            var clause = new TerminalClause("one", Operator.GREATER_THAN, "one");
            var mutableClause = new SingleMutableClause(clause);
            Assert.Same(mutableClause, mutableClause.Copy());
        }

        [Fact]
        public void TestToString()
        {
            var clause = new TerminalClause("one", Operator.GREATER_THAN, 1);
            var mutableClause = new SingleMutableClause(clause);
            Assert.Equal(clause.ToString(), mutableClause.ToString());
        }
    }
}