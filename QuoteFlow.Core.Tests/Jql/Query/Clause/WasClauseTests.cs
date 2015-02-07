using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Core.Jql.Query.Clause;
using QuoteFlow.Core.Jql.Query.History;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Query.Clause
{
    public class WasClauseTests
    {
        public static SingleValueOperand _singleValueOperand = new SingleValueOperand("test");

        public class TheCtor
        {
            [Fact]
            public void Throws_If_NameIsNull()
            {
                var ex = Assert.Throws<ArgumentNullException>(() => new WasClause(null, Operator.IN, _singleValueOperand, null));
                Assert.Equal("field", ex.ParamName);
            }
        }

        [Fact]
        public void NeverContains_Any_Subclauses()
        {
            Assert.True(!new WasClause("testField", Operator.WAS, _singleValueOperand, null).Clauses.Any());
        }

        [Fact]
        public void Test_ToString()
        {
            var clause = new WasClause("testField", Operator.WAS, _singleValueOperand, null);
            Assert.Equal("{testField was \"test\"}", clause.ToString());
        }

        public class TheAcceptMethod
        {
            [Fact]
            public void TestVisit()
            {
                var visitor = new ClauseVisitorHelper(this, false);
                var clause = new WasClause("testField", Operator.WAS, _singleValueOperand, null).Accept(visitor);
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
                    return FailVisitor();
                }

                public object Visit(IWasClause clause)
                {
                    VisitCalled = true;
                    return null;
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
        public void ToString_SeperatesOperand_And_Predicate_BySpace()
        {
            var wasClause = new WasClause("status", Operator.WAS, new SingleValueOperand("closed"),
                new TerminalHistoryPredicate(Operator.ON, new SingleValueOperand(3500000)));
            Assert.Equal("{status was \"closed\" on 3500000}", wasClause.ToString());
        }
    }
}
