using System.Collections.Generic;
using System.Linq;
using Moq;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;
using QuoteFlow.Core.Jql.Context;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Context
{
    public class QueryContextVisitorTests
    {
        public class RootClauseTests
        {
            [Fact]
            public void TestRootClause()
            {
                var andClause = new AndClause(new TerminalClause("blah", Operator.GREATER_THAN, "blah"));
                var notClause = new NotClause(new TerminalClause("blah", Operator.GREATER_THAN, "blah"));
                var orClause = new OrClause(new TerminalClause("blah", Operator.GREATER_THAN, "blah"));
                var termClause = new TerminalClause("blah", Operator.GREATER_THAN, "blah");

                AssertRootCalled(andClause, CreateCatalog(1));
                AssertRootCalled(orClause, CreateCatalog(34));
                AssertRootCalled(notClause, CreateCatalog(333));
                AssertRootCalled(termClause, CreateCatalog(12));
            }

            private static void AssertRootCalled(IClause clause, IClauseContext expectedContext)
            {
                var rootVisitor = new RootContextVisitor(expectedContext);
                var contextResult = clause.Accept(rootVisitor);
                Assert.Equal(clause, rootVisitor.GetCalledClause());
                Assert.Equal(expectedContext, contextResult.FullContext);
                Assert.Equal(expectedContext, contextResult.SimpleContext);
            }

            private class RootContextVisitor : QueryContextVisitor
            {
                private IClause _calledWith;
                private readonly IClauseContext _result;

                public RootContextVisitor(IClauseContext result)
                    : base(null, null, null)
                {
                    _result = result;
                }

                public override ContextResult CreateContext(IClause clause)
                {
                    _calledWith = clause;
                    return new ContextResult(_result, _result);
                }

                public IClause GetCalledClause()
                {
                    return _calledWith;
                }
            }
        }

        public class OrClauseTests
        {
            [Fact]

            public void TestOr()
            {
                var termClause1 = new TerminalClause("blah1", Operator.GREATER_THAN, "val1");
                var termClause2 = new TerminalClause("blah2", Operator.GREATER_THAN, "val2");
                var andClause = new OrClause(termClause1, termClause2);

                var clauseContext1 = CreateCatalog(78);

                var clauseContextFactory = new Mock<IClauseContextFactory>();
                clauseContextFactory.Setup(x => x.GetClauseContext(null, termClause1)).Returns(clauseContext1);
                clauseContextFactory.Setup(x => x.GetClauseContext(null, termClause2)).Returns(clauseContext1);

                var clauseHandler = new MockClauseHandler()
                    .SetContextFactory(clauseContextFactory.Object)
                    .SetInformation(new MockClauseInformation(new ClauseNames("blah1")));
                var clauseHandler2 = new MockClauseHandler()
                    .SetContextFactory(clauseContextFactory.Object)
                    .SetInformation(new MockClauseInformation(new ClauseNames("blah2")));

                var searchHandlerManager = new Mock<ISearchHandlerManager>();
                searchHandlerManager.Setup(x => x.GetClauseHandler((User) null, "blah1"))
                    .Returns(new HashSet<IClauseHandler> {clauseHandler});
                searchHandlerManager.Setup(x => x.GetClauseHandler((User)null, "blah2"))
                    .Returns(new HashSet<IClauseHandler> { clauseHandler2 });

                var contextSetUtil = new Mock<ContextSetUtil>();

                var visitor = new QueryContextVisitor(null, contextSetUtil.Object, searchHandlerManager.Object);
                var result = andClause.Accept(visitor);

                var expectedResult = new QueryContextVisitor.ContextResult(clauseContext1, ClauseContext.CreateGlobalClauseContext());
                
                Assert.Equal(expectedResult, result);
            }
        }

        private static IClauseContext CreateCatalog(params int[] ids)
        {
            var contexts = ids.Select(
                    id => new CatalogManufacturerContext(new CatalogContext(id), AllManufacturersContext.Instance))
                    .ToList();
            return new ClauseContext(contexts);
        }
    }
}