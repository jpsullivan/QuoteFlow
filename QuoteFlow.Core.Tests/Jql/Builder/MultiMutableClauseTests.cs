﻿using System;
using Moq;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Core.Jql.Builder;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Builder
{
    public class MultiMutableClauseTests
    {
        [Fact]
        public void TestCombine()
        {
            IMutableClause clause = new SingleMutableClause(new TerminalClause("who", Operator.LESS_THAN, "cares?"));
            IMutableClause clause2 = new SingleMutableClause(new TerminalClause("not", Operator.GREATER_THAN, "me"));
            IMutableClause clause3 = new SingleMutableClause(new TerminalClause("never", Operator.NOT_EQUALS, "true"));
            IMutableClause clause4 = new SingleMutableClause(new TerminalClause("life", Operator.LIKE, "death"));
            IMutableClause clause5 = new SingleMutableClause(new TerminalClause("manager", Operator.LESS_THAN, "monkey"));

            IMutableClause expectedClause = new MultiMutableClause<IMutableClause>(BuilderOperator.AND, clause, clause2);

            IMutableClause actualClause = new MultiMutableClause<IMutableClause>(BuilderOperator.AND, clause);
            actualClause = actualClause.Combine(BuilderOperator.AND, clause2);
            Assert.Equal(expectedClause, actualClause);

            expectedClause = new MultiMutableClause<IMutableClause>(BuilderOperator.AND, clause, clause2, clause3);
            actualClause = actualClause.Combine(BuilderOperator.AND, clause3);
            Assert.Equal(expectedClause, actualClause);

            expectedClause = new MultiMutableClause<IMutableClause>(BuilderOperator.AND, clause, clause2, clause3);
            expectedClause = new MultiMutableClause<IMutableClause>(BuilderOperator.OR, expectedClause, clause4);
            actualClause = actualClause.Combine(BuilderOperator.OR, clause4);
            Assert.Equal(expectedClause, actualClause);

            expectedClause = new MultiMutableClause<IMutableClause>(BuilderOperator.AND, clause, clause2, clause3);
            expectedClause = new MultiMutableClause<IMutableClause>(BuilderOperator.OR, expectedClause, clause4, clause5);
            actualClause = actualClause.Combine(BuilderOperator.OR, clause5);
            Assert.Equal(expectedClause, actualClause);

            expectedClause = new MultiMutableClause<IMutableClause>(BuilderOperator.AND, clause, clause2, clause3);
            expectedClause = new NotMutableClause(new MultiMutableClause<IMutableClause>(BuilderOperator.OR, expectedClause, clause4, clause5));
            actualClause = actualClause.Combine(BuilderOperator.NOT, null);
            Assert.Equal(expectedClause, actualClause);
        }

        [Fact]
        public void TestAsClause()
        {
            IClause whoCaresClause = new TerminalClause("who", Operator.LESS_THAN, "cares?");
            IClause lessMe = new TerminalClause("not", Operator.GREATER_THAN, "me");
            IMutableClause clause = new SingleMutableClause(whoCaresClause);
            IMutableClause clause2 = new SingleMutableClause(lessMe);
            IMutableClause clause3 = new Mock<IMutableClause>().Object;

            IMutableClause mutableClause = new MultiMutableClause<IMutableClause>(BuilderOperator.AND, clause);
            Assert.Same(whoCaresClause, mutableClause.AsClause());

            mutableClause = new MultiMutableClause<IMutableClause>(BuilderOperator.OR, clause);
            Assert.Same(whoCaresClause, mutableClause.AsClause());

            mutableClause = new MultiMutableClause<IMutableClause>(BuilderOperator.AND, clause, clause2);
            Assert.Equal(new AndClause(whoCaresClause, lessMe), mutableClause.AsClause());

            mutableClause = new MultiMutableClause<IMutableClause>(BuilderOperator.OR, clause, clause2);
            Assert.Equal(new OrClause(whoCaresClause, lessMe), mutableClause.AsClause());

            mutableClause = new MultiMutableClause<IMutableClause>(BuilderOperator.OR, clause3);
            Assert.Null(mutableClause.AsClause());

            mutableClause = new MultiMutableClause<IMutableClause>(BuilderOperator.AND, clause3, clause2);
            Assert.Equal(lessMe, mutableClause.AsClause());

            mutableClause = new MultiMutableClause<IMutableClause>(BuilderOperator.AND, clause3, clause2, clause);
            Assert.Equal(new AndClause(lessMe, whoCaresClause), mutableClause.AsClause());
        }

        [Fact]
        public void TestCopyWithAnd()
        {
            MockMutableClause stringClause1 = new MockMutableClause(null);
            var clause1 = new Mock<IMutableClause>();
            clause1.Setup(x => x.Copy()).Returns(stringClause1);

            MockMutableClause stringClause2 = new MockMutableClause(null);
            var clause2 = new Mock<IMutableClause>();
            clause2.Setup(x => x.Copy()).Returns(stringClause2);

            MockMutableClause stringClause3 = new MockMutableClause(null);
            var clause3 = new Mock<IMutableClause>();
            clause3.Setup(x => x.Copy()).Returns(stringClause3);
            
            var multiClause = new MultiMutableClause<IMutableClause>(BuilderOperator.AND, clause1.Object, clause2.Object, clause3.Object);
            IMutableClause actualCopy = multiClause.Copy();
            IMutableClause expectedCopy = new MultiMutableClause<IMutableClause>(BuilderOperator.AND, stringClause1, stringClause2, stringClause3);

            Assert.Equal(expectedCopy, actualCopy);

            multiClause = new MultiMutableClause<IMutableClause>(BuilderOperator.OR, clause1.Object);
            actualCopy = multiClause.Copy();
            expectedCopy = new MultiMutableClause<IMutableClause>(BuilderOperator.OR, stringClause1);

            Assert.Equal(expectedCopy, actualCopy);

            clause1.Verify();
            clause2.Verify();
            clause3.Verify();
        }
    }
}