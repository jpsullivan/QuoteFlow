using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Core.Jql.Clause;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Clause
{
    public class DeMorgansVisitorTests
    {
        [Fact]
        public void TestNoNots()
        {
            var terminalClause1 = new TerminalClause("blarg", Operator.EQUALS, "blarg");
            var terminalClause2 = new TerminalClause("blargal", Operator.LESS_THAN, "blargal");
            var terminalClause3 = new TerminalClause("garg", Operator.GREATER_THAN_EQUALS, "garg");
            var terminalClause4 = new TerminalClause("gargle", Operator.LIKE, "gargle");

            var orClause1 = new OrClause(terminalClause1, terminalClause2);
            var orClause2 = new OrClause(terminalClause3, terminalClause4);

            var topClause = new AndClause(orClause1, orClause2);

            var deMorgansVisitor = new DeMorgansVisitor();
            var result = deMorgansVisitor.Visit(topClause);
            Assert.Equal(topClause, result);
        }

        [Fact]
        public void TestNotSingleEquals()
        {
            var equalsClause = new TerminalClause("blarg", Operator.EQUALS, "blarg");
            var notClause = new NotClause(equalsClause);

            var expectedClause = new TerminalClause("blarg", Operator.NOT_EQUALS, "blarg");

            var deMorgansVisitor = new DeMorgansVisitor();
            var result = deMorgansVisitor.Visit(notClause);
            Assert.Equal(expectedClause, result);
        }

        [Fact]
        public void TestNotSingleNotEquals()
        {
            var equalsClause = new TerminalClause("blarg", Operator.NOT_EQUALS, "blarg");
            var notClause = new NotClause(equalsClause);

            var expectedClause = new TerminalClause("blarg", Operator.EQUALS, "blarg");

            var deMorgansVisitor = new DeMorgansVisitor();
            var result = deMorgansVisitor.Visit(notClause);
            Assert.Equal(expectedClause, result);
        }

        [Fact]
        public void TestNotSingleGreaterThan()
        {
            var equalsClause = new TerminalClause("blarg", Operator.GREATER_THAN, "blarg");
            var notClause = new NotClause(equalsClause);

            var expectedClause = new TerminalClause("blarg", Operator.LESS_THAN_EQUALS, "blarg");

            var deMorgansVisitor = new DeMorgansVisitor();
            var result = deMorgansVisitor.Visit(notClause);
            Assert.Equal(expectedClause, result);
        }

        [Fact]
        public void TestNotSingleGreaterThanEquals()
        {
            var equalsClause = new TerminalClause("blarg", Operator.GREATER_THAN_EQUALS, "blarg");
            var notClause = new NotClause(equalsClause);

            var expectedClause = new TerminalClause("blarg", Operator.LESS_THAN, "blarg");

            var deMorgansVisitor = new DeMorgansVisitor();
            var result = deMorgansVisitor.Visit(notClause);
            Assert.Equal(expectedClause, result);
        }

        [Fact]
        public void TestNotSingleLessThan()
        {
            var equalsClause = new TerminalClause("blarg", Operator.LESS_THAN, "blarg");
            var notClause = new NotClause(equalsClause);

            var expectedClause = new TerminalClause("blarg", Operator.GREATER_THAN_EQUALS, "blarg");

            var deMorgansVisitor = new DeMorgansVisitor();
            var result = deMorgansVisitor.Visit(notClause);
            Assert.Equal(expectedClause, result);
        }

        [Fact]
        public void TestNotSingleLessThanEquals()
        {
            var equalsClause = new TerminalClause("blarg", Operator.LESS_THAN_EQUALS, "blarg");
            var notClause = new NotClause(equalsClause);

            var expectedClause = new TerminalClause("blarg", Operator.GREATER_THAN, "blarg");

            var deMorgansVisitor = new DeMorgansVisitor();
            var result = deMorgansVisitor.Visit(notClause);
            Assert.Equal(expectedClause, result);
        }

        [Fact]
        public void TestNotSingleLike()
        {
            var equalsClause = new TerminalClause("blarg", Operator.LIKE, "blarg");
            var notClause = new NotClause(equalsClause);

            var expectedClause = new TerminalClause("blarg", Operator.NOT_LIKE, "blarg");

            var deMorgansVisitor = new DeMorgansVisitor();
            var result = deMorgansVisitor.Visit(notClause);
            Assert.Equal(expectedClause, result);
        }

        [Fact]
        public void TestNotSingleNotLike()
        {
            var equalsClause = new TerminalClause("blarg", Operator.NOT_LIKE, "blarg");
            var notClause = new NotClause(equalsClause);

            var expectedClause = new TerminalClause("blarg", Operator.LIKE, "blarg");

            var deMorgansVisitor = new DeMorgansVisitor();
            var result = deMorgansVisitor.Visit(notClause);
            Assert.Equal(expectedClause, result);
        }

        [Fact]
        public void TestNotSingleIs()
        {
            var equalsClause = new TerminalClause("blarg", Operator.IS, "blarg");
            var notClause = new NotClause(equalsClause);

            var expectedClause = new TerminalClause("blarg", Operator.IS_NOT, "blarg");

            var deMorgansVisitor = new DeMorgansVisitor();
            var result = deMorgansVisitor.Visit(notClause);
            Assert.Equal(expectedClause, result);
        }

        [Fact]
        public void TestNotSingleIn()
        {
            var equalsClause = new TerminalClause("blarg", Operator.IN, "blarg");
            var notClause = new NotClause(equalsClause);

            var expectedClause = new TerminalClause("blarg", Operator.NOT_IN, "blarg");

            var deMorgansVisitor = new DeMorgansVisitor();
            var result = deMorgansVisitor.Visit(notClause);
            Assert.Equal(expectedClause, result);
        }

        [Fact]
        public void TestNotSingleIsNot()
        {
            var equalsClause = new TerminalClause("blarg", Operator.IS_NOT, "blarg");
            var notClause = new NotClause(equalsClause);

            var expectedClause = new TerminalClause("blarg", Operator.IS, "blarg");

            var deMorgansVisitor = new DeMorgansVisitor();
            var result = deMorgansVisitor.Visit(notClause);
            Assert.Equal(expectedClause, result);
        }

        [Fact]
        public void TestNotSingleNotIn()
        {
            var equalsClause = new TerminalClause("blarg", Operator.NOT_IN, "blarg");
            var notClause = new NotClause(equalsClause);

            var expectedClause = new TerminalClause("blarg", Operator.IN, "blarg");

            var deMorgansVisitor = new DeMorgansVisitor();
            var result = deMorgansVisitor.Visit(notClause);
            Assert.Equal(expectedClause, result);
        }

        [Fact]
        public void TestNotAnd()
        {
            var terminalClause1 = new TerminalClause("blarg", Operator.EQUALS, "blarg");
            var terminalClause2 = new TerminalClause("blargal", Operator.LESS_THAN, "blargal");
            var andClause = new AndClause(terminalClause1, terminalClause2);
            var notClause = new NotClause(andClause);

            var terminalClause3 = new TerminalClause("blarg", Operator.NOT_EQUALS, "blarg");
            var terminalClause4 = new TerminalClause("blargal", Operator.GREATER_THAN_EQUALS, "blargal");
            var expectedClause = new OrClause(terminalClause3, terminalClause4);

            var deMorgansVisitor = new DeMorgansVisitor();
            var result = deMorgansVisitor.Visit(notClause);
            Assert.Equal(expectedClause, result);
        }

        [Fact]
        public void TestNotOr()
        {
            var terminalClause1 = new TerminalClause("blarg", Operator.GREATER_THAN_EQUALS, "blarg");
            var terminalClause2 = new TerminalClause("blargal", Operator.NOT_EQUALS, "blargal");
            var andClause = new AndClause(terminalClause1, terminalClause2);
            var notClause = new NotClause(andClause);

            var terminalClause3 = new TerminalClause("blarg", Operator.LESS_THAN, "blarg");
            var terminalClause4 = new TerminalClause("blargal", Operator.EQUALS, "blargal");
            var expectedClause = new OrClause(terminalClause3, terminalClause4);

            var deMorgansVisitor = new DeMorgansVisitor();
            var result = deMorgansVisitor.Visit(notClause);
            Assert.Equal(expectedClause, result);
        }

        [Fact]
        public void TestNotAndWithLike()
        {
            var terminalClause1 = new TerminalClause("blarg", Operator.LIKE, "blarg");
            var terminalClause2 = new TerminalClause("blargal", Operator.LESS_THAN, "blargal");
            var andClause = new AndClause(terminalClause1, terminalClause2);
            var notClause = new NotClause(andClause);

            var terminalClause3 = new TerminalClause("blarg", Operator.NOT_LIKE, "blarg");
            var terminalClause4 = new TerminalClause("blargal", Operator.GREATER_THAN_EQUALS, "blargal");
            var expectedClause = new OrClause(terminalClause3, terminalClause4);

            var deMorgansVisitor = new DeMorgansVisitor();
            var result = deMorgansVisitor.Visit(notClause);
            Assert.Equal(expectedClause, result);
        }

        [Fact]
        public void TestTwoLevel()
        {
            var terminalClause1 = new TerminalClause("blarg", Operator.LIKE, "blarg");
            var terminalClause2 = new TerminalClause("blargal", Operator.LESS_THAN, "blargal");
            var notAndClause = new NotClause(new AndClause(terminalClause1, terminalClause2));

            var terminalClause3 = new TerminalClause("blarg", Operator.GREATER_THAN_EQUALS, "blarg");
            var terminalClause4 = new TerminalClause("blargal", Operator.EQUALS, "blargal");
            var notOrClause = new NotClause(new OrClause(terminalClause3, terminalClause4));
            var topClause = new AndClause(notOrClause, notAndClause);

            var notLiked = new TerminalClause("blarg", Operator.NOT_LIKE, "blarg");
            var expected1 = new TerminalClause("blargal", Operator.GREATER_THAN_EQUALS, "blargal");
            var expOrClause = new OrClause(notLiked, expected1);

            var expected3 = new TerminalClause("blarg", Operator.LESS_THAN, "blarg");
            var expected4 = new TerminalClause("blargal", Operator.NOT_EQUALS, "blargal");
            var expAndClause = new AndClause(expected3, expected4);
            var expectedTopClause = new AndClause(expAndClause, expOrClause);
            
            var deMorgansVisitor = new DeMorgansVisitor();
            var result = deMorgansVisitor.Visit(topClause);
            Assert.Equal(expectedTopClause, result);
        }

        [Fact]
        public void TestTwoNots()
        {
            var terminalClause = new TerminalClause("blargal", Operator.LESS_THAN, "blargal");
            var notClause = new NotClause(new NotClause(terminalClause));

            var deMorgansVisitor = new DeMorgansVisitor();
            var result = deMorgansVisitor.Visit(notClause);
            Assert.Equal(terminalClause, result);
        }

        [Fact]
        public void TestThreeNots()
        {
            var terminalClause = new TerminalClause("blargal", Operator.LESS_THAN, "blargal");
            var expectedClause = new TerminalClause("blargal", Operator.GREATER_THAN_EQUALS, "blargal");
            var notClause = new NotClause(new NotClause(new NotClause(terminalClause)));

            var deMorgansVisitor = new DeMorgansVisitor();
            var result = deMorgansVisitor.Visit(notClause);
            Assert.Equal(expectedClause, result);
        }

        [Fact]
        public void TestTwoLevelNot()
        {
            var terminalClause1 = new TerminalClause("blarg", Operator.LIKE, "blarg");
            var terminalClause2 = new TerminalClause("blargal", Operator.LESS_THAN, "blargal");
            var andClause = new AndClause(terminalClause1, terminalClause2);

            var terminalClause3 = new TerminalClause("garg", Operator.GREATER_THAN_EQUALS, "garg");
            var terminalClause4 = new TerminalClause("gargle", Operator.EQUALS, "gargle");
            var notOrClause = new NotClause(new OrClause(terminalClause3, terminalClause4));
            var topClause = new NotClause(new AndClause(andClause, notOrClause));

            var notLiked = new TerminalClause("blarg", Operator.NOT_LIKE, "blarg");
            var expected1 = new TerminalClause("blargal", Operator.GREATER_THAN_EQUALS, "blargal");
            var expOrClause1 = new OrClause(notLiked, expected1);

            var expected3 = new TerminalClause("garg", Operator.GREATER_THAN_EQUALS, "garg");
            var expected4 = new TerminalClause("gargle", Operator.EQUALS, "gargle");
            var expOrClause2 = new OrClause(expected3, expected4);
            var expectedTopClause = new OrClause(expOrClause1, expOrClause2);

            var deMorgansVisitor = new DeMorgansVisitor();
            var result = deMorgansVisitor.Visit(topClause);
            Assert.Equal(expectedTopClause, result);
        }
    }
}