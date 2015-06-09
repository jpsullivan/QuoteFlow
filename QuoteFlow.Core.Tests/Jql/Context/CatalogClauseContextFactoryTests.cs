using System.Collections.Generic;
using Moq;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Jql.Resolver;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Jql.Context;
using QuoteFlow.Core.Tests.Jql.Validator;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Context
{
    public class CatalogClauseContextFactoryTests
    {
        private static IJqlOperandResolver _jqlOperandResolver;
        private static INameResolver<Catalog> _catalogResolver;
        private static ICatalogService _catalogService;
        private static User _theUser = null;

        public CatalogClauseContextFactoryTests()
        {
            _jqlOperandResolver = MockJqlOperandResolver.CreateSimpleSupport();
            _catalogResolver = new Mock<INameResolver<Catalog>>().Object;
            _catalogService = new Mock<ICatalogService>().Object;
        }

        public class TheGetClauseContextMethod
        {
            [Fact]
            public void BadOperator()
            {
                var catalogClause = new TerminalClause("catalog", Operator.GREATER_THAN, 12345);
                var clauseContextFactory = new CatalogClauseContextFactory(_catalogService, _jqlOperandResolver, _catalogResolver);

                var expectedContext = ClauseContext.CreateGlobalClauseContext();
                var clauseContext = clauseContextFactory.GetClauseContext(_theUser, catalogClause);
                Assert.Equal(expectedContext, clauseContext);
            }

            [Fact]
            public void ForEmpty()
            {
                var catalogClause = new TerminalClause("catalog", Operator.GREATER_THAN, EmptyOperand.Empty);
                var clauseContextFactory = new CatalogClauseContextFactory(_catalogService, _jqlOperandResolver, _catalogResolver);

                var expectedContext = ClauseContext.CreateGlobalClauseContext();
                var clauseContext = clauseContextFactory.GetClauseContext(_theUser, catalogClause);
                Assert.Equal(expectedContext, clauseContext);
            }

            [Fact]
            public void ForNullLiterals()
            {
                var catalogClause = new TerminalClause("catalog", Operator.GREATER_THAN, 12345);
                var jqlOperandResolver = new Mock<IJqlOperandResolver>();
                jqlOperandResolver.Setup(x => x.GetValues(_theUser, catalogClause.Operand, catalogClause))
                    .Returns((IEnumerable<QueryLiteral>) null);

                var clauseContextFactory = new CatalogClauseContextFactory(_catalogService, jqlOperandResolver.Object, _catalogResolver);

                var expectedContext = ClauseContext.CreateGlobalClauseContext();
                var clauseContext = clauseContextFactory.GetClauseContext(_theUser, catalogClause);
                Assert.Equal(expectedContext, clauseContext);
            }

            [Fact]
            public void EmptyInList()
            {
                var catalogClause = new TerminalClause("catalog", Operator.IN, new MultiValueOperand(new EmptyOperand()));
                var jqlOperandResolver = MockJqlOperandResolver.CreateSimpleSupport();
                var clauseContextFactory = new CatalogClauseContextFactory(_catalogService, jqlOperandResolver, _catalogResolver);

                var expectedContext = ClauseContext.CreateGlobalClauseContext();
                var clauseContext = clauseContextFactory.GetClauseContext(_theUser, catalogClause);
                Assert.Equal(expectedContext, clauseContext);
            }
        }
    }
}