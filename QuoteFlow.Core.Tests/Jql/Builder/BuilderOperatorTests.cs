using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Core.Jql.Builder;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Builder
{
    public class BuilderOperatorTests
    {
        [Fact]
        public void TestOrder()
        {
            var expectedOrder = new List<BuilderOperator>
            {
                BuilderOperator.None,
                BuilderOperator.LPAREN,
                BuilderOperator.RPAREN,
                BuilderOperator.OR,
                BuilderOperator.AND,
                BuilderOperator.NOT
            };

            // even though this is a set, its order is defined by the declaration of the Enum.
            var actualOrder = EnumExtensions.GetValues<BuilderOperator>().ToList();
            Assert.Equal(expectedOrder.Count, actualOrder.Count);

            IEnumerator<BuilderOperator> actualIterator = actualOrder.GetEnumerator();
            foreach (BuilderOperator @operator in expectedOrder)
            {
                if (actualIterator.MoveNext())
                {
                    Assert.Equal(@operator, actualIterator.Current);
                }
            }
        }

        [Fact]
        public void TestAndCombine()
        {
            IMutableClause left = new SingleMutableClause(new TerminalClause("dont", Operator.EQUALS, "care"));
            IMutableClause right = new SingleMutableClause(new TerminalClause("carefactor", Operator.EQUALS, "0"));

            IMutableClause actualClause = BuilderOperator.AND.CreateClauseForOperator(left, right);
            Assert.Equal(new MultiMutableClause<IMutableClause>(BuilderOperator.AND, left, right), actualClause);
        }

        [Fact]
        public void TestOrCombine()
        {
            IMutableClause left = new SingleMutableClause(new TerminalClause("dont", Operator.EQUALS, "care"));
            IMutableClause right = new SingleMutableClause(new TerminalClause("carefactor", Operator.EQUALS, "0"));

            IMutableClause actualClause = BuilderOperator.OR.CreateClauseForOperator(left, right);
            Assert.Equal(new MultiMutableClause<IMutableClause>(BuilderOperator.OR, left, right), actualClause);
        }

        [Fact]
        public void TestNotCombine()
        {
            IMutableClause left = new SingleMutableClause(new TerminalClause("dont", Operator.EQUALS, "care"));
            IMutableClause right = new SingleMutableClause(new TerminalClause("carefactor", Operator.EQUALS, "0"));

            IMutableClause actualClause = BuilderOperator.NOT.CreateClauseForOperator(left, right);
            Assert.Equal(new NotMutableClause(left), actualClause);

            actualClause = BuilderOperator.NOT.CreateClauseForOperator(left, null);
            Assert.Equal(new NotMutableClause(left), actualClause);
        }
    }
}