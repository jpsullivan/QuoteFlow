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
    public class OrClauseTests
    {
        private static readonly IClause MockClause = new Mock<IClause>().Object;

        public class TheCtor
        {
            [Fact]
            public void Throws_If_ClauseIsEmpty()
            {
                var ex = Assert.Throws<ArgumentException>(() => new OrClause(new List<IClause>()));
                Assert.Equal("You can not construct an OrClause without any child clauses.", ex.Message);
            }
        }

        [Fact]
        public void Name_Returns_OR()
        {
            Assert.Equal("OR", new OrClause(MockClause).Name);
        }

        public class TheToStringMethod
        {
            [Fact]
            public void ProducesTheCorrectOutput()
            {
                var terminalClause1 = new TerminalClause("testField", Operator.EQUALS, new SingleValueOperand("test"));
                var terminalClause2 = new TerminalClause("anotherField", Operator.GREATER_THAN, new SingleValueOperand("other"));
                var orClause = new OrClause(terminalClause1, terminalClause2);
                Assert.Equal("{testField = \"test\"} OR {anotherField > \"other\"}", orClause.ToString());
            }

            [Fact]
            public void ProducesTheCorrectOutput_WithPrecedence()
            {
                var terminalClause1 = new TerminalClause("testField", Operator.EQUALS, new SingleValueOperand("test"));
                var terminalClause2 = new TerminalClause("anotherField", Operator.GREATER_THAN, new SingleValueOperand("other")); 
                var notClause = new NotClause(new TerminalClause("thirdField", Operator.GREATER_THAN, new SingleValueOperand("other")));
                var andClause = new AndClause(new TerminalClause("fourthField", Operator.GREATER_THAN, new SingleValueOperand("other")), new TerminalClause("fifthField", Operator.GREATER_THAN, new SingleValueOperand("other")));
                var orClause = new OrClause(terminalClause1, terminalClause2, notClause, andClause);
                Assert.Equal("{testField = \"test\"} OR {anotherField > \"other\"} OR NOT {thirdField > \"other\"} OR {fourthField > \"other\"} AND {fifthField > \"other\"}", orClause.ToString());
            }

            [Fact]
            public void ProducesTheCorrectOutput_WithPrecedence_NestedOr()
            {
                var  terminalClause1 = new TerminalClause("testField", Operator.EQUALS, new SingleValueOperand("test"));
                var subOrClause = new OrClause(new TerminalClause("fourthField", Operator.GREATER_THAN, new SingleValueOperand("other")), new TerminalClause("fifthField", Operator.GREATER_THAN, new SingleValueOperand("other")));
                var orClause = new OrClause(terminalClause1, subOrClause);
                Assert.Equal("{testField = \"test\"} OR {fourthField > \"other\"} OR {fifthField > \"other\"}", orClause.ToString());
            }
        }

        public class TheAcceptMethod
        {
            [Fact]
            public void TestVisit()
            {
                var visitor = new ClauseVisitorHelper(this, false);
                var clause = new OrClause(MockClause).Accept(visitor);
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
                    return FailVisitor();
                }

                public object Visit(NotClause notClause)
                {
                    return FailVisitor();
                }

                public object Visit(OrClause orClause)
                {
                    VisitCalled = true;
                    return null;
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
            var orClause = new OrClause(new List<IClause> { terminalClause1, terminalClause2 });
            Assert.Equal(2, orClause.Clauses.Count());
        }
    }
}
