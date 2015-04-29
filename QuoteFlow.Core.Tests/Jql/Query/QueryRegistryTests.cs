using System;
using System.Collections.Generic;
using Moq;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;
using QuoteFlow.Core.Jql.Query;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Query
{
    public class QueryRegistryTests
    {
        private const User Anonymous = null;
        private const string ClauseName = "clauseName";

        private Mock<IClauseQueryFactory> _mockClauseQueryFactory;
        private Mock<IClauseHandler> _mockClauseHandler;
        private Mock<ISearchHandlerManager> _mockSearchHandlerManager;

        private IQueryCreationContext _queryCreationContext;

        public QueryRegistryTests()
        {
            _mockClauseQueryFactory = new Mock<IClauseQueryFactory>();
            _mockClauseHandler = new Mock<IClauseHandler>();
            _mockSearchHandlerManager = new Mock<ISearchHandlerManager>();

            _queryCreationContext = new QueryCreationContext(Anonymous);
        }

        [Fact]
        public void TestGetClauseQueryFactory()
        {
            _mockClauseHandler.Setup(x => x.Factory).Returns(_mockClauseQueryFactory.Object);
            _mockSearchHandlerManager.Setup(x => x.GetClauseHandler(Anonymous, ClauseName))
                .Returns(new List<IClauseHandler> {_mockClauseHandler.Object});

            var queryRegistry = GetFixture();
            var result = queryRegistry.GetClauseQueryFactory(_queryCreationContext,
                new TerminalClause(ClauseName, Operator.IN, "value"));

            Assert.Contains(_mockClauseQueryFactory.Object, result);

            // make sure that we didn't try without security applied
            _mockSearchHandlerManager.Verify(x => x.GetClauseHandler(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void TestGetClauseQueryFactoryOverrideSecurity()
        {
            _queryCreationContext = new QueryCreationContext(Anonymous, true);

            _mockClauseHandler.Setup(x => x.Factory).Returns(_mockClauseQueryFactory.Object);
            _mockSearchHandlerManager.Setup(x => x.GetClauseHandler(ClauseName))
                .Returns(new List<IClauseHandler> { _mockClauseHandler.Object });

            var queryRegistry = GetFixture();
            var result = queryRegistry.GetClauseQueryFactory(_queryCreationContext,
                new TerminalClause(ClauseName, Operator.IN, "value"));

            Assert.Contains(_mockClauseQueryFactory.Object, result);

            // make sure that we didn't try without security applied
            _mockSearchHandlerManager.Verify(x => x.GetClauseHandler(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void TestGetClauseQueryFactory_BadArgs()
        {
            var queryRegistry = GetFixture();
            Assert.Throws<ArgumentNullException>(() => queryRegistry.GetClauseQueryFactory(_queryCreationContext, null));
        }

        private QueryRegistry GetFixture()
        {
            return new QueryRegistry(_mockSearchHandlerManager.Object);
        }
    }
}