using System.Collections.Generic;
using Moq;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Operator;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Jql.Util;
using QuoteFlow.Api.Models;
using QuoteFlow.Core.Jql.Context;
using QuoteFlow.Core.Tests.Jql.Query.Operand;
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
        public void TestGetClauseContext()
        {
            var operand = new SingleValueOperand("blah");
            var clause = new TerminalClause("blah", Operator.EQUALS, operand);

            var mockJqlOperandResolver = new Mock<IJqlOperandResolver>();
            mockJqlOperandResolver.Setup(x => x.GetValues(_theUser, operand, clause))
                .Returns(new List<QueryLiteral>
                {
                    SimpleLiteralFactory.CreateLiteral("sku"),
                    SimpleLiteralFactory.CreateLiteral(10)
                });

            var mockJqlAssetSupport = new Mock<IJqlAssetSupport>();
            mockJqlAssetSupport.Setup(x => x.GetCatalogManufacturerPairsByIds(new HashSet<int> {10}))
                .Returns(new HashSet<KeyValuePair<int, string>> {new KeyValuePair<int, string>(11, "11")});
            mockJqlAssetSupport.Setup(x => x.GetCatalogManufacturerPairsBySkus(new HashSet<string> { "sku" }))
                .Returns(new HashSet<KeyValuePair<int, string>> { new KeyValuePair<int, string>(10, "10") });

            var factory = new AssetIdClauseContextFactory(mockJqlAssetSupport.Object, mockJqlOperandResolver.Object,
                OperatorClasses.EqualityAndRelational);
            var result = factory.GetClauseContext(_theUser, clause);
            var expectedResult = new ClauseContext(new List<CatalogManufacturerContext>
            {
                new CatalogManufacturerContext(new CatalogContext(10), new ManufacturerContext(10)),
                new CatalogManufacturerContext(new CatalogContext(11), new ManufacturerContext(11))
            });

            Assert.Equal(expectedResult, result);
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