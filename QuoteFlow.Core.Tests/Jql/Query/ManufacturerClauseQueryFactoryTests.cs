using Moq;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Jql.Query;
using QuoteFlow.Core.Jql.Resolver;
using QuoteFlow.Core.Tests.Jql.Validator;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Query
{
    public class ManufacturerClauseQueryFactoryTests
    {
        [Fact]
        public void TestUnsupportedOperators()
        {
            Operator[] invalidOperators = 
            {
                Operator.GREATER_THAN, 
                Operator.GREATER_THAN_EQUALS, 
                Operator.LESS_THAN, 
                Operator.LESS_THAN_EQUALS,
                Operator.LIKE
            };

            var manufacturerResolver = new ManufacturerResolver(new Mock<IManufacturerService>().Object);
            var singleValueOperand = new SingleValueOperand("testOperand");
            
            foreach (var @operator in invalidOperators)
            {
                var manufacturerClauseQueryFactory = new ManufacturerClauseQueryFactory(manufacturerResolver, MockJqlOperandResolver.CreateSimpleSupport());
                var terminalClause = new TerminalClause("manufacturer", @operator, singleValueOperand);

                var result = manufacturerClauseQueryFactory.GetQuery(null, terminalClause);
                Assert.Equal(QueryFactoryResult.CreateFalseResult(), result);
            }
        }
    }
}