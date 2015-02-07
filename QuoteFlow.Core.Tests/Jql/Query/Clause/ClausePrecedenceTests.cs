using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Query.Clause
{
    public class ClausePrecedenceTests
    {
        [Fact]
        public void Test_Get_And()
        {
            var clausePrecedence = ClausePrecedenceHelper.GetPrecedence(new AndClause(new TerminalClause("blah", Operator.EQUALS, "blah")));
            Assert.Equal(ClausePrecedence.AND, clausePrecedence);
        }

        [Fact]
        public void Test_Get_Or()
        {
            var clausePrecedence = ClausePrecedenceHelper.GetPrecedence(new OrClause(new TerminalClause("blah", Operator.EQUALS, "blah")));
            Assert.Equal(ClausePrecedence.OR, clausePrecedence);
        }

        [Fact]
        public void Test_Get_Not()
        {
            var clausePrecedence = ClausePrecedenceHelper.GetPrecedence(new NotClause(new TerminalClause("blah", Operator.EQUALS, "blah")));
            Assert.Equal(ClausePrecedence.NOT, clausePrecedence);
        }

        [Fact]
        public void Test_Get_Terminal()
        {
            var clausePrecedence = ClausePrecedenceHelper.GetPrecedence(new TerminalClause("blah", Operator.EQUALS, "blah"));
            Assert.Equal(ClausePrecedence.TERMINAL, clausePrecedence);
        }

        [Fact]
        public void Test_PrecedenceOrder()
        {
            Assert.True((int) ClausePrecedence.TERMINAL > (int) ClausePrecedence.NOT);
            Assert.True((int) ClausePrecedence.TERMINAL > (int) ClausePrecedence.AND);
            Assert.True((int) ClausePrecedence.TERMINAL > (int) ClausePrecedence.OR);

            Assert.True((int) ClausePrecedence.NOT > (int) ClausePrecedence.AND);
            Assert.True((int) ClausePrecedence.NOT > (int) ClausePrecedence.OR);

            Assert.True((int) ClausePrecedence.AND > (int) ClausePrecedence.OR);
        }
    }
}
