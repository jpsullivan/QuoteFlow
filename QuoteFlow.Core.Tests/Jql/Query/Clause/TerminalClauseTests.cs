using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Query.Clause
{
    public class TerminalClauseTests
    {
        public static SingleValueOperand _singleValueOperand = new SingleValueOperand("test");

        public class TheCtor
        {
            [Fact]
            public void Throws_If_NameIsNull()
            {
                var ex = Assert.Throws<ArgumentNullException>(() => new TerminalClause(null, Operator.IN, _singleValueOperand));
                Assert.Equal("name", ex.ParamName);
            }
        }

        [Fact]
        public void NeverContains_SubClauses()
        {
            Assert.True(!new TerminalClause("testField", Operator.EQUALS, _singleValueOperand).Clauses.Any());
        }

        public class TheToStringMethod
        {
            [Fact]
            public void ProducesTheCorrectOutput()
            {
                var clause = new TerminalClause("testField", Operator.EQUALS, _singleValueOperand);
                Assert.Equal("{testField = \"test\"}", clause.ToString());
            }

            [Fact]
            public void ProducesTheCorrectOutput_WithProperties()
            {
                var properties = new List<Property>();
                properties.Add(new Property(new List<string> {"k1", "k2"}, new List<string> {"ref1", "ref2"}));
                var clause = new TerminalClause("testField", Operator.EQUALS, _singleValueOperand, properties);

                Assert.Equal("{testField[k1.k2].ref1.ref2 = \"test\"}", clause.ToString());
            }
        }

        public class TheAcceptMethod
        {
            [Fact]
            public void TestVisit()
            {
                var visitor = new ClauseVisitorHelper(this, false);
                var clause = new TerminalClause("testField", Operator.EQUALS, _singleValueOperand).Accept(visitor);
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
                    return FailVisitor();
                }

                public object Visit(ITerminalClause clause)
                {
                    VisitCalled = true;
                    return null;
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
