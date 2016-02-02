using Moq;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Operator;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Util;
using QuoteFlow.Api.Models;
using QuoteFlow.Core.Jql.Context;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Context
{
    public class AssetIdClauseContextFactoryTests
    {
        private User _theUser = null;

        [Fact]
        public void TestGetClauseContextRelationalNotAllowed()
        {
            var jqlOperandResolver = new Mock<IJqlOperandResolver>().Object;
            var jqlAssetSupport = new Mock<IJqlAssetSupport>().Object;

            var expectedResult = ClauseContext.CreateGlobalClauseContext();

            foreach (var @operator in OperatorClasses.RelationalOnlyOperators)
            {
                var factory = new AssetIdClauseContextFactory(jqlAssetSupport, jqlOperandResolver, OperatorClasses.EqualityOperators);
                var result = factory.GetClauseContext(_theUser, new TerminalClause("blah", @operator, "blah"));

                Assert.Equal(expectedResult, result);
            }
        }

        [Fact]
        public void TestGetClauseContextInequality()
        {
            var jqlOperandResolver = new Mock<IJqlOperandResolver>().Object;
            var jqlAssetSupport = new Mock<IJqlAssetSupport>().Object;

            var expectedResult = ClauseContext.CreateGlobalClauseContext();

            var factory = new AssetIdClauseContextFactory(jqlAssetSupport, jqlOperandResolver, OperatorClasses.EqualityAndRelational);
            var result = factory.GetClauseContext(_theUser, new TerminalClause("blah", Operator.NOT_EQUALS, "blah"));

            Assert.Equal(expectedResult, result);
        }
    }
}