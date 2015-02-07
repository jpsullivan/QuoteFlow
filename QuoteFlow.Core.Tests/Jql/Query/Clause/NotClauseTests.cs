using System;
using System.Linq;
using Moq;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Query.Clause
{
    public class NotClauseTests
    {
        private static readonly IClause MockClause = new Mock<IClause>().Object;

        public class TheCtor
        {
            [Fact]
            public void Throws_If_SubclauseIsEmpty()
            {
                var ex = Assert.Throws<ArgumentNullException>(() => new NotClause(null));
                Assert.Equal("subClause", ex.ParamName);
            }
        }

        [Fact]
        public void Name_Returns_NOT()
        {
            Assert.Equal("NOT", new NotClause(MockClause).Name);
        }

        public class TheToStringMethod
        {
            [Fact]
            public void ProducesTheCorrectOutput()
            {
                var  terminalClause = new TerminalClause("testField", Operator.EQUALS, new SingleValueOperand("test"));
                var notClause = new NotClause(terminalClause);
                Assert.Equal("NOT {testField = \"test\"}", notClause.ToString());
            }

            [Fact]
            public void ProducesTheCorrectOutput_WithAndPrecedence()
            {
                var andClause =
                    new AndClause(
                        new TerminalClause("fourthField", Operator.GREATER_THAN, new SingleValueOperand("other")),
                        new TerminalClause("fifthField", Operator.GREATER_THAN, new SingleValueOperand("other")));
                var notClause = new NotClause(andClause);
                
                Assert.Equal("NOT ( {fourthField > \"other\"} AND {fifthField > \"other\"} )", notClause.ToString());
            }

            [Fact]
            public void ProducesTheCorrectOutput_WithNotPrecedence()
            {
                var subNotClause = new NotClause(new TerminalClause("fourthField", Operator.GREATER_THAN, new SingleValueOperand("other")));
                var notClause = new NotClause(subNotClause);
                Assert.Equal("NOT NOT {fourthField > \"other\"}", notClause.ToString());
            }
        }

        public class TheClauseProperty
        {
            [Fact]
            public void GetsTheClause()
            {
                var terminalClause = new TerminalClause("testField", Operator.EQUALS, new SingleValueOperand("test"));
                var notClause = new NotClause(terminalClause);
                Assert.Equal(terminalClause, notClause.SubClause);
                Assert.Equal(1, notClause.Clauses.Count());
                Assert.Equal(terminalClause, notClause.Clauses.First());
            }
        }

        public class TheAcceptMethod
        {
            [Fact]
            public void TestVisit()
            {
                var visitor = new ClauseVisitorHelper(this, false);
                var clause = new NotClause(MockClause).Accept(visitor);
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
                    VisitCalled = true;
                    return null;
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
    }
}
