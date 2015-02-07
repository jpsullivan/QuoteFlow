using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Query.Clause
{
    public class AndClauseTests
    {
        private static readonly IClause MockClause = new Mock<IClause>().Object;

        public class TheCtor
        {
            [Fact]
            public void Throws_If_ClausesIsEmpty()
            {
                var ex = Assert.Throws<ArgumentException>(() => new AndClause(new List<IClause>()));
                Assert.Equal("You can not construct an AndClause without any child clauses.", ex.Message);
            }
        }

        [Fact]
        public void Name_Returns_AND()
        {
            Assert.Equal("AND", new AndClause(MockClause).Name);
        }

        public class TheToStringMethod
        {
            [Fact]
            public void ProducesTheCorrectOutput()
            {
                var terminalClause1 = new TerminalClause("testField", Operator.EQUALS, new SingleValueOperand("test"));
                var terminalClause2 = new TerminalClause("anotherField", Operator.GREATER_THAN, new SingleValueOperand("other"));
                var andClause = new AndClause(terminalClause1, terminalClause2);
                Assert.Equal("{testField = \"test\"} AND {anotherField > \"other\"}", andClause.ToString());
            }

            [Fact]
            public void ProducesTheCorrectOutput_WithPrecedence()
            {
                var terminalClause1 = new TerminalClause("testField", Operator.EQUALS, new SingleValueOperand("test"));
                var terminalClause2 = new TerminalClause("anotherField", Operator.GREATER_THAN, new SingleValueOperand("other"));
                var notClause = new NotClause(new TerminalClause("thirdField", Operator.GREATER_THAN, new SingleValueOperand("other")));
                var orClause = new OrClause(new TerminalClause("fourthField", Operator.GREATER_THAN, new SingleValueOperand("other")), new TerminalClause("fifthField", Operator.GREATER_THAN, new SingleValueOperand("other")));
                AndClause andClause = new AndClause(terminalClause1, terminalClause2, notClause, orClause);
                Assert.Equal("{testField = \"test\"} AND {anotherField > \"other\"} AND NOT {thirdField > \"other\"} AND ( {fourthField > \"other\"} OR {fifthField > \"other\"} )", andClause.ToString());
            }

            [Fact]
            public void ProducesTheCorrectOutput_WithPrecedence_NestedAnd()
            {
                var terminalClause1 = new TerminalClause("testField", Operator.EQUALS, new SingleValueOperand("test"));
                var subAndClause = new AndClause(new TerminalClause("fourthField", Operator.GREATER_THAN, new SingleValueOperand("other")), new TerminalClause("fifthField", Operator.GREATER_THAN, new SingleValueOperand("other")));
                AndClause andClause = new AndClause(terminalClause1, subAndClause);
                Assert.Equal("{testField = \"test\"} AND {fourthField > \"other\"} AND {fifthField > \"other\"}", andClause.ToString());
            }
        }

        public class TheAcceptMethod
        {
            [Fact]
            public void TestVisit()
            {
                var visitor = new ClauseVisitorHelper(this, false);
                var clause = new AndClause(MockClause).Accept(visitor);
                Assert.True(visitor.VisitCalled);
            }

            private class ClauseVisitorHelper : IClauseVisitor<object>
            {
                public TheAcceptMethod OuterInstance { get; set; }
                public bool VisitCalled { get; set; }

                public ClauseVisitorHelper(TheAcceptMethod outerInstance, bool visitCalled)
                {
                    OuterInstance = outerInstance;
                    VisitCalled = visitCalled;
                }

                public object Visit(AndClause andClause)
                {
                    VisitCalled = true;
                    return null;
                }

                public object Visit(NotClause notClause)
                {
                    return FailVisitor();
                }

                public object Visit(OrClause orClause)
                {
                    return FailVisitor();
                }

                public object Visit(ITerminalClause clause)
                {
                    return FailVisitor();
                }

                public object Visit(IWasClause clause)
                {
                    return FailVisitor();
                }

                public object Visit(IChangedClause clause)
                {
                    return FailVisitor();
                }
            }

            private static object FailVisitor()
            {
                Assert.True(false, "Should not be called");
                return null;
            }
        }

        [Fact]
        public void Test_HappyPath()
        {
            var terminalClause1 = new TerminalClause("testField", Operator.EQUALS, new SingleValueOperand("test"));
            var terminalClause2 = new TerminalClause("anotherField", Operator.GREATER_THAN, new SingleValueOperand("other"));
            var andClause = new AndClause(new List<IClause> { terminalClause1, terminalClause2 });
            Assert.Equal(2, andClause.Clauses.Count());
        }
    }
}
