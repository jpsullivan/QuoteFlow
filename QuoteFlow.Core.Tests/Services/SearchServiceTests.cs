using Moq;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Parser;
using QuoteFlow.Api.Jql.Util;
using QuoteFlow.Core.Jql.Context;
using QuoteFlow.Core.Jql.Validator;
using QuoteFlow.Core.Services;
using Xunit;

namespace QuoteFlow.Core.Tests.Services
{
    public class SearchServiceTests
    {
        public class TheGetQueryContextMethod
        {
            [Fact]
            public void ReturnsNull()
            {
                
            }
        }
    }

    public class TestableSearchService : SearchService
    {
        public Mock<IJqlQueryParser> MockJqlQueryParser { get; set; }
        public Mock<IJqlStringSupport> MockJqlStringSupport { get; set; }
        public Mock<IJqlOperandResolver> MockJqlOperandResolver { get; set; }
        public Mock<ValidatorVisitor.ValidatorVisitorFactory> MockValidatorVisitorFactory { get; set; }
        public Mock<ISearchHandlerManager> MockSearchHandlerManager { get; set; }
        public Mock<QueryContextVisitor.QueryContextVisitorFactory> MockQueryContextVisitorFactory { get; set; } 

        public TestableSearchService(MockBehavior behavior = MockBehavior.Default)
        {
            JqlQueryParser = (MockJqlQueryParser = new Mock<IJqlQueryParser>(behavior)).Object;
            JqlStringSupport = (MockJqlStringSupport = new Mock<IJqlStringSupport>(behavior)).Object;
            JqlOperandResolver = (MockJqlOperandResolver = new Mock<IJqlOperandResolver>(behavior)).Object;
            ValidatorVisitorFactory = (MockValidatorVisitorFactory = new Mock<ValidatorVisitor.ValidatorVisitorFactory>(behavior)).Object;
            SearchHandlerManager = (MockSearchHandlerManager = new Mock<ISearchHandlerManager>(behavior)).Object;
            QueryContextVisitorFactory = (MockQueryContextVisitorFactory = new Mock<QueryContextVisitor.QueryContextVisitorFactory>(behavior)).Object;
        }
    }
}
