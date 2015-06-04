using System.Collections.Generic;
using Moq;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Parser;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Util;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Asset.Search;
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

        public class TheGetSimpleQueryContextMethod
        {
            [Fact]
            public void Test()
            {
                var clause = new Mock<IClause>();
                var queryContextVisitor = new QueryContextVisitor(null, null);

                var searchService = new TestableSearchService();

                var searchQuery = new Query(clause.Object);
                var fullContext = new ClauseContext(new List<ICatalogManufacturerContext>());
                var simpleContext = new ClauseContext(new List<ICatalogManufacturerContext>());
                var contextResult = new QueryContextVisitor.ContextResult(fullContext, simpleContext);

                searchService.MockQueryCache.Setup(x => x.GetSimpleQueryContextCache(null, searchQuery))
                    .Returns((IQueryContext) null);

                searchService.QueryCache.SetQueryContextCache(null, searchQuery, new QueryContext(fullContext));
                searchService.QueryCache.SetSimpleQueryContextCache(null, searchQuery, new QueryContext(simpleContext));

                searchService.MockQueryContextVisitorFactory.Setup(x => x.CreateVisitor(null)).Returns(queryContextVisitor);

                clause.Setup(x => x.Accept(queryContextVisitor)).Returns(contextResult);

                searchService.GetSimpleQueryContext(null, searchQuery);
                clause.Verify();
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
        public Mock<QueryContextConverter> MockQueryContextConverter { get; set; }
        public Mock<IQueryCache> MockQueryCache { get; set; }
        public Mock<ISearchProvider> MockSearchProvider { get; set; }

        public TestableSearchService(MockBehavior behavior = MockBehavior.Default)
        {
            JqlQueryParser = (MockJqlQueryParser = new Mock<IJqlQueryParser>(behavior)).Object;
            JqlStringSupport = (MockJqlStringSupport = new Mock<IJqlStringSupport>(behavior)).Object;
            JqlOperandResolver = (MockJqlOperandResolver = new Mock<IJqlOperandResolver>(behavior)).Object;
            ValidatorVisitorFactory = (MockValidatorVisitorFactory = new Mock<ValidatorVisitor.ValidatorVisitorFactory>(behavior)).Object;
            SearchHandlerManager = (MockSearchHandlerManager = new Mock<ISearchHandlerManager>(behavior)).Object;
            QueryContextVisitorFactory = (MockQueryContextVisitorFactory = new Mock<QueryContextVisitor.QueryContextVisitorFactory>(behavior)).Object;
            QueryContextConverter = (MockQueryContextConverter = new Mock<QueryContextConverter>(behavior)).Object;
            QueryCache = (MockQueryCache = new Mock<IQueryCache>(behavior)).Object;
            SearchProvider = (MockSearchProvider = new Mock<ISearchProvider>(behavior)).Object;
        }
    }
}
