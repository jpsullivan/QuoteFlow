using System.Collections.Generic;
using System.Linq;
using Moq;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Constants;
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

            [Fact]
            public void TestOrUnion()
            {
                var termClause1 = new TerminalClause("blah1", Operator.GREATER_THAN, "val1");
                var termClause2 = new TerminalClause("blah2", Operator.GREATER_THAN, "val2");
                var termClause2Negated = new TerminalClause("blah2", Operator.LESS_THAN_EQUALS, "val2");
                var orClause = new OrClause(termClause1, new NotClause(termClause2));

                var clauseContext1 = CreateCatalog(18202);
                var catalogManufacturerContext = new CatalogManufacturerContext(new CatalogContext(10), new ManufacturerContext(10));
                var clauseContext2 = new ClauseContext(new List<ICatalogManufacturerContext> {catalogManufacturerContext});

                
                var clauseContextFactory = new Mock<IClauseContextFactory>();
                clauseContextFactory.Setup(x => x.GetClauseContext(null, termClause1)).Returns(clauseContext1);
                clauseContextFactory.Setup(x => x.GetClauseContext(null, termClause2Negated)).Returns(clauseContext2);

                var clauseHandler = new MockClauseHandler()
                    .SetContextFactory(clauseContextFactory.Object)
                    .SetInformation(new MockClauseInformation(new ClauseNames("blah1")));
                var clauseHandler2 = new MockClauseHandler()
                    .SetContextFactory(clauseContextFactory.Object)
                    .SetInformation(new MockClauseInformation(new ClauseNames("blah2")));

                var searchHandlerManager = new Mock<ISearchHandlerManager>();
                searchHandlerManager.Setup(x => x.GetClauseHandler((User)null, "blah1"))
                    .Returns(new HashSet<IClauseHandler> { clauseHandler });
                searchHandlerManager.Setup(x => x.GetClauseHandler((User)null, "blah2"))
                    .Returns(new HashSet<IClauseHandler> { clauseHandler2 });

                var contextSetUtil = new Mock<ContextSetUtil>();

                var clauseContext3 = CreateCatalog(15, 6, 0x3993);
                contextSetUtil.Setup(x => x.Union(new HashSet<IClauseContext> {clauseContext1, clauseContext2}))
                    .Returns(clauseContext3);

                var visitor = new QueryContextVisitor(null, contextSetUtil.Object, searchHandlerManager.Object);
                var result = orClause.Accept(visitor);
                var expectedResult = new QueryContextVisitor.ContextResult(clauseContext3, ClauseContext.CreateGlobalClauseContext());

                Assert.Equal(expectedResult, result);
            }

            [Fact]
            public void TestOrUnionSimpleAndComplexDifferent()
            {
                var termClause1 = new TerminalClause("blah1", Operator.GREATER_THAN, "val1");
                var termClause2 = new TerminalClause("catalog", Operator.GREATER_THAN, "val2");
                var termClause3 = new TerminalClause("manufacturer", Operator.GREATER_THAN, "val2");
                var orClause = new OrClause(termClause1, termClause2, termClause3);

                var clauseContext1 = CreateCatalog(78);
                var catalogManufacturerContext = new CatalogManufacturerContext(new CatalogContext(10), new ManufacturerContext(10));
                var clauseContext2 = new ClauseContext(new List<ICatalogManufacturerContext> { catalogManufacturerContext });
                var clauseContext3 = CreateCatalog(678, 292);
                
                var clauseContextFactory = new Mock<IClauseContextFactory>();
                clauseContextFactory.Setup(x => x.GetClauseContext(null, termClause1)).Returns(clauseContext1);
                clauseContextFactory.Setup(x => x.GetClauseContext(null, termClause2)).Returns(clauseContext2);
                clauseContextFactory.Setup(x => x.GetClauseContext(null, termClause3)).Returns(clauseContext3);

                var clauseHandler = new MockClauseHandler()
                    .SetContextFactory(clauseContextFactory.Object)
                    .SetInformation(new MockClauseInformation(new ClauseNames("blah1")));
                var clauseHandler2 = new MockClauseHandler()
                    .SetContextFactory(clauseContextFactory.Object)
                    .SetInformation(new MockClauseInformation(SystemSearchConstants.ForCatalog().JqlClauseNames));
                var clauseHandler3 = new MockClauseHandler()
                    .SetContextFactory(clauseContextFactory.Object)
                    .SetInformation(new MockClauseInformation(SystemSearchConstants.ForManufacturer().JqlClauseNames));

                var searchHandlerManager = new Mock<ISearchHandlerManager>();
                searchHandlerManager.Setup(x => x.GetClauseHandler((User)null, "blah1"))
                    .Returns(new HashSet<IClauseHandler> { clauseHandler });
                searchHandlerManager.Setup(x => x.GetClauseHandler((User)null, "catalog"))
                    .Returns(new HashSet<IClauseHandler> { clauseHandler2 });
                searchHandlerManager.Setup(x => x.GetClauseHandler((User)null, "manufacturer"))
                    .Returns(new HashSet<IClauseHandler> { clauseHandler3 });

                var contextSetUtil = new Mock<ContextSetUtil>();

                // the end result of visitation is at a minimum the Global context; never an empty context.
                var clauseContextComplex = CreateCatalog(2, 4, 6);
                var clauseContextSimple = CreateCatalog(2, 4, 6);

                contextSetUtil.Setup(
                    x => x.Union(new HashSet<IClauseContext> {clauseContext1, clauseContext2, clauseContext3}))
                    .Returns(clauseContextComplex);
                contextSetUtil.Setup(
                    x =>
                        x.Union(new HashSet<IClauseContext>
                        {
                            ClauseContext.CreateGlobalClauseContext(),
                            clauseContext2,
                            clauseContext3
                        })).Returns(clauseContextSimple);

                var visitor = new QueryContextVisitor(null, contextSetUtil.Object, searchHandlerManager.Object);
                var result = orClause.Accept(visitor);
                var expectedResult = new QueryContextVisitor.ContextResult(clauseContextComplex, clauseContextSimple);

                Assert.Equal(expectedResult, result);
            }

            [Fact]
            public void TestOrUnionSimpleAndComplexSame()
            {
                var termClause2 = new TerminalClause("catalog", Operator.GREATER_THAN, "val2");
                var termClause3 = new TerminalClause("manufacturer", Operator.GREATER_THAN, "val2");
                var orClause = new OrClause(termClause2, termClause3);

                var catalogManufacturerContext = new CatalogManufacturerContext(new CatalogContext(10), new ManufacturerContext(10));
                var clauseContext2 = new ClauseContext(new List<ICatalogManufacturerContext> { catalogManufacturerContext });
                var clauseContext3 = CreateCatalog(678, 292);

                var clauseContextFactory = new Mock<IClauseContextFactory>();
                clauseContextFactory.Setup(x => x.GetClauseContext(null, termClause2)).Returns(clauseContext2);
                clauseContextFactory.Setup(x => x.GetClauseContext(null, termClause3)).Returns(clauseContext3);

                var clauseHandler2 = new MockClauseHandler()
                    .SetContextFactory(clauseContextFactory.Object)
                    .SetInformation(new MockClauseInformation(SystemSearchConstants.ForCatalog().JqlClauseNames));
                var clauseHandler3 = new MockClauseHandler()
                    .SetContextFactory(clauseContextFactory.Object)
                    .SetInformation(new MockClauseInformation(SystemSearchConstants.ForManufacturer().JqlClauseNames));

                var searchHandlerManager = new Mock<ISearchHandlerManager>();
                searchHandlerManager.Setup(x => x.GetClauseHandler((User)null, "catalog"))
                    .Returns(new HashSet<IClauseHandler> { clauseHandler2 });
                searchHandlerManager.Setup(x => x.GetClauseHandler((User)null, "manufacturer"))
                    .Returns(new HashSet<IClauseHandler> { clauseHandler3 });

                var contextSetUtil = new Mock<ContextSetUtil>();

                // the end result of visitation is at a minimum the Global context; never an empty context.
                var clauseContextComplex = CreateCatalog(2, 4, 6);

                contextSetUtil.Setup(
                    x => x.Union(new HashSet<IClauseContext> { clauseContext2, clauseContext3 }))
                    .Returns(clauseContextComplex);

                var visitor = new QueryContextVisitor(null, contextSetUtil.Object, searchHandlerManager.Object);
                var result = orClause.Accept(visitor);
                var expectedResult = new QueryContextVisitor.ContextResult(clauseContextComplex, clauseContextComplex);

                Assert.Equal(expectedResult, result);
            }
        }

        public class AndClauseTests
        {
            [Fact]
            public void TestAnd()
            {
                var termClause1 = new TerminalClause("blah1", Operator.GREATER_THAN, "val1");
                var termClause2 = new TerminalClause("blah2", Operator.GREATER_THAN, "val2");
                var andClause = new AndClause(termClause1, termClause2);

                var clauseContext1 = CreateCatalog(78);

                var clauseContextFactory = new Mock<IClauseContextFactory>();
                clauseContextFactory.Setup(x => x.GetClauseContext(null, termClause1)).Returns(clauseContext1);
                clauseContextFactory.Setup(x => x.GetClauseContext(null, termClause2)).Returns(clauseContext1);

                var clauseHandler1 = new MockClauseHandler()
                    .SetContextFactory(clauseContextFactory.Object)
                    .SetInformation(new MockClauseInformation(new ClauseNames("blah1")));
                var clauseHandler2 = new MockClauseHandler()
                    .SetContextFactory(clauseContextFactory.Object)
                    .SetInformation(new MockClauseInformation(new ClauseNames("blah2")));

                var searchHandlerManager = new Mock<ISearchHandlerManager>();
                searchHandlerManager.Setup(x => x.GetClauseHandler((User)null, "blah1"))
                    .Returns(new HashSet<IClauseHandler> { clauseHandler1 });
                searchHandlerManager.Setup(x => x.GetClauseHandler((User)null, "blah2"))
                    .Returns(new HashSet<IClauseHandler> { clauseHandler2 });

                var contextSetUtil = new Mock<ContextSetUtil>();

                var visitor = new QueryContextVisitor(null, contextSetUtil.Object, searchHandlerManager.Object);
                var result = andClause.Accept(visitor);

                var expectedResult = new QueryContextVisitor.ContextResult(clauseContext1, ClauseContext.CreateGlobalClauseContext());

                Assert.Equal(expectedResult, result);
            }

            [Fact]
            public void TestAndIntersect()
            {
                // we are checking the negation here
                var termClause1 = new TerminalClause("blah1", Operator.GREATER_THAN, "val1");
                var termClause1negated = new TerminalClause("blah1", Operator.LESS_THAN_EQUALS, "val1");
                var termClause2 = new TerminalClause("blah2", Operator.GREATER_THAN, "val2");
                var termClause2negated = new TerminalClause("blah2", Operator.LESS_THAN_EQUALS, "val2");
                var andClause = new NotClause(new OrClause(termClause1, termClause2));

                var clauseContext1 = CreateCatalog(78);
                var catalogManufacturerContext = new CatalogManufacturerContext(new CatalogContext(10), new ManufacturerContext(10));
                var clauseContext2 = new ClauseContext(new List<ICatalogManufacturerContext> { catalogManufacturerContext });

                var clauseContextFactory = new Mock<IClauseContextFactory>();
                clauseContextFactory.Setup(x => x.GetClauseContext(null, termClause1negated)).Returns(clauseContext1);
                clauseContextFactory.Setup(x => x.GetClauseContext(null, termClause2negated)).Returns(clauseContext2);

                var clauseHandler1 = new MockClauseHandler()
                    .SetContextFactory(clauseContextFactory.Object)
                    .SetInformation(new MockClauseInformation(new ClauseNames("blah1")));
                var clauseHandler2 = new MockClauseHandler()
                    .SetContextFactory(clauseContextFactory.Object)
                    .SetInformation(new MockClauseInformation(new ClauseNames("blah2")));

                var searchHandlerManager = new Mock<ISearchHandlerManager>();
                searchHandlerManager.Setup(x => x.GetClauseHandler((User)null, "blah1"))
                    .Returns(new HashSet<IClauseHandler> { clauseHandler1 });
                searchHandlerManager.Setup(x => x.GetClauseHandler((User)null, "blah2"))
                    .Returns(new HashSet<IClauseHandler> { clauseHandler2 });

                var contextSetUtil = new Mock<ContextSetUtil>();

                // the end result of visitation is at a minimum the Global context; never an empty context.
                var clauseContextComplex = CreateCatalog(2, 4, 6);

                contextSetUtil.Setup(
                    x => x.Intersect(new HashSet<IClauseContext> { clauseContext1, clauseContext2 }))
                    .Returns(clauseContextComplex);

                var visitor = new QueryContextVisitor(null, contextSetUtil.Object, searchHandlerManager.Object);
                var result = andClause.Accept(visitor);
                var expectedResult = new QueryContextVisitor.ContextResult(clauseContextComplex, ClauseContext.CreateGlobalClauseContext());

                Assert.Equal(expectedResult, result);
            }

            [Fact]
            public void TestAndIntersect_SimpleAndComplex_Different()
            {
                var termClause1 = new TerminalClause("blah1", Operator.GREATER_THAN, "val1");
                var termClause2 = new TerminalClause("catalog", Operator.GREATER_THAN, "val2");
                var termClause3 = new TerminalClause("manufacturer", Operator.GREATER_THAN, "val2");
                var andClause = new AndClause(termClause1, termClause2, termClause3);

                var clauseContext1 = CreateCatalog(78);
                var catalogManufacturerContext = new CatalogManufacturerContext(new CatalogContext(10), new ManufacturerContext(10));
                var clauseContext2 = new ClauseContext(new List<ICatalogManufacturerContext> { catalogManufacturerContext });
                var clauseContext3 = CreateCatalog(678, 292);

                var clauseContextFactory = new Mock<IClauseContextFactory>();
                clauseContextFactory.Setup(x => x.GetClauseContext(null, termClause1)).Returns(clauseContext1);
                clauseContextFactory.Setup(x => x.GetClauseContext(null, termClause2)).Returns(clauseContext2);
                clauseContextFactory.Setup(x => x.GetClauseContext(null, termClause3)).Returns(clauseContext3);

                var clauseHandler = new MockClauseHandler()
                    .SetContextFactory(clauseContextFactory.Object)
                    .SetInformation(new MockClauseInformation(new ClauseNames("blah1")));
                var clauseHandler2 = new MockClauseHandler()
                    .SetContextFactory(clauseContextFactory.Object)
                    .SetInformation(new MockClauseInformation(SystemSearchConstants.ForCatalog().JqlClauseNames));
                var clauseHandler3 = new MockClauseHandler()
                    .SetContextFactory(clauseContextFactory.Object)
                    .SetInformation(new MockClauseInformation(SystemSearchConstants.ForManufacturer().JqlClauseNames));

                var searchHandlerManager = new Mock<ISearchHandlerManager>();
                searchHandlerManager.Setup(x => x.GetClauseHandler((User)null, "blah1"))
                    .Returns(new HashSet<IClauseHandler> { clauseHandler });
                searchHandlerManager.Setup(x => x.GetClauseHandler((User)null, "catalog"))
                    .Returns(new HashSet<IClauseHandler> { clauseHandler2 });
                searchHandlerManager.Setup(x => x.GetClauseHandler((User)null, "manufacturer"))
                    .Returns(new HashSet<IClauseHandler> { clauseHandler3 });

                var contextSetUtil = new Mock<ContextSetUtil>();

                // the end result of visitation is at a minimum the Global context; never an empty context.
                var clauseContextComplex = CreateCatalog(2, 4, 6);
                var clauseContextSimple = CreateCatalog(2, 4, 6);

                contextSetUtil.Setup(
                    x => x.Intersect(new HashSet<IClauseContext> { clauseContext1, clauseContext2, clauseContext3 }))
                    .Returns(clauseContextComplex);
                contextSetUtil.Setup(
                    x =>
                        x.Intersect(new HashSet<IClauseContext>
                        {
                            ClauseContext.CreateGlobalClauseContext(),
                            clauseContext2,
                            clauseContext3
                        })).Returns(clauseContextSimple);

                var visitor = new QueryContextVisitor(null, contextSetUtil.Object, searchHandlerManager.Object);
                var result = andClause.Accept(visitor);
                var expectedResult = new QueryContextVisitor.ContextResult(clauseContextComplex, clauseContextSimple);

                Assert.Equal(expectedResult, result);
            }

            [Fact]
            public void TestAndIntersectSimpleAndComplexSame()
            {
                var termClause1 = new TerminalClause("blah1", Operator.GREATER_THAN, "val1");
                var termClause2 = new TerminalClause("catalog", Operator.GREATER_THAN, "val2");
                var termClause3 = new TerminalClause("manufacturer", Operator.GREATER_THAN, "val2");
                var andClause = new AndClause(termClause1, termClause2, termClause3);

                var catalogManufacturerContext = new CatalogManufacturerContext(new CatalogContext(10), new ManufacturerContext(10));
                var clauseContext1 = ClauseContext.CreateGlobalClauseContext();
                var clauseContext2 = new ClauseContext(new List<ICatalogManufacturerContext> { catalogManufacturerContext });
                var clauseContext3 = CreateCatalog(678, 292);

                var clauseContextFactory = new Mock<IClauseContextFactory>();
                clauseContextFactory.Setup(x => x.GetClauseContext(null, termClause1)).Returns(clauseContext1);
                clauseContextFactory.Setup(x => x.GetClauseContext(null, termClause2)).Returns(clauseContext2);
                clauseContextFactory.Setup(x => x.GetClauseContext(null, termClause3)).Returns(clauseContext3);

                var clauseHandler1 = new MockClauseHandler()
                    .SetContextFactory(clauseContextFactory.Object)
                    .SetInformation(new MockClauseInformation(new ClauseNames("blah1")));
                var clauseHandler2 = new MockClauseHandler()
                    .SetContextFactory(clauseContextFactory.Object)
                    .SetInformation(new MockClauseInformation(SystemSearchConstants.ForCatalog().JqlClauseNames));
                var clauseHandler3 = new MockClauseHandler()
                    .SetContextFactory(clauseContextFactory.Object)
                    .SetInformation(new MockClauseInformation(SystemSearchConstants.ForManufacturer().JqlClauseNames));

                var searchHandlerManager = new Mock<ISearchHandlerManager>();
                searchHandlerManager.Setup(x => x.GetClauseHandler((User)null, "blah1"))
                    .Returns(new HashSet<IClauseHandler> { clauseHandler1 });
                searchHandlerManager.Setup(x => x.GetClauseHandler((User)null, "catalog"))
                    .Returns(new HashSet<IClauseHandler> { clauseHandler2 });
                searchHandlerManager.Setup(x => x.GetClauseHandler((User)null, "manufacturer"))
                    .Returns(new HashSet<IClauseHandler> { clauseHandler3 });

                var contextSetUtil = new Mock<ContextSetUtil>();

                // the end result of visitation is at a minimum the Global context; never an empty context.
                var clauseContextComplex = CreateCatalog(2, 4, 6);

                contextSetUtil.Setup(
                    x => x.Intersect(new HashSet<IClauseContext> { clauseContext1, clauseContext2, clauseContext3 }))
                    .Returns(clauseContextComplex);

                var visitor = new QueryContextVisitor(null, contextSetUtil.Object, searchHandlerManager.Object);
                var result = andClause.Accept(visitor);
                var expectedResult = new QueryContextVisitor.ContextResult(clauseContextComplex, clauseContextComplex);

                Assert.Equal(expectedResult, result);
            }
        }

        public class TerminalClauseTests
        {
            [Fact]
            public void TestTerminalClause()
            {
                var termClause1 = new TerminalClause("blah1", Operator.GREATER_THAN, "val1");

                var clauseContext1 = CreateCatalog(394748494);

                var clauseContextFactory = new Mock<IClauseContextFactory>();
                clauseContextFactory.Setup(x => x.GetClauseContext(null, termClause1)).Returns(clauseContext1);

                var clauseHandler = new MockClauseHandler()
                    .SetContextFactory(clauseContextFactory.Object)
                    .SetInformation(new MockClauseInformation(new ClauseNames("blah1")));

                var searchHandlerManager = new Mock<ISearchHandlerManager>();
                searchHandlerManager.Setup(x => x.GetClauseHandler((User)null, "blah1"))
                    .Returns(new HashSet<IClauseHandler> { clauseHandler });

                var contextSetUtil = new Mock<ContextSetUtil>();

                var visitorFactory = new QueryContextVisitor.QueryContextVisitorFactory(contextSetUtil.Object, searchHandlerManager.Object);
                var visitor = visitorFactory.CreateVisitor(null);
                var result = termClause1.Accept(visitor);

                var expectedResult = new QueryContextVisitor.ContextResult(clauseContext1, ClauseContext.CreateGlobalClauseContext());

                Assert.Equal(expectedResult, result);
            }

            [Fact]
            public void TestTerminalClause_NoHandlers()
            {
                var termClause1 = new TerminalClause("blah1", Operator.GREATER_THAN, "val1");
                
                var searchHandlerManager = new Mock<ISearchHandlerManager>();
                searchHandlerManager.Setup(x => x.GetClauseHandler((User)null, "blah1"))
                    .Returns(new HashSet<IClauseHandler>());

                var contextSetUtil = new Mock<ContextSetUtil>();

                var visitorFactory = new QueryContextVisitor.QueryContextVisitorFactory(contextSetUtil.Object, searchHandlerManager.Object);
                var visitor = visitorFactory.CreateVisitor(null);
                var result = termClause1.Accept(visitor);

                var expectedResult = new QueryContextVisitor.ContextResult(ClauseContext.CreateGlobalClauseContext(), ClauseContext.CreateGlobalClauseContext());

                Assert.Equal(expectedResult, result);
            }

            [Fact]
            public void TestTerminalClause_MultipleHandlers()
            {
                var termClause1 = new TerminalClause("gin", Operator.EQUALS, "1");

                var clauseContext1 = CreateCatalog(1);
                var clauseContext2 = CreateCatalog(2);
                var clauseContext3 = CreateCatalog(3);

                var clauseContextFactory = new Mock<IClauseContextFactory>();
                clauseContextFactory.Setup(x => x.GetClauseContext(null, termClause1)).Returns(clauseContext1);

                var clauseContextFactory2 = new Mock<IClauseContextFactory>();
                clauseContextFactory2.Setup(x => x.GetClauseContext(null, termClause1)).Returns(clauseContext2);

                var clauseHandler = new MockClauseHandler()
                    .SetContextFactory(clauseContextFactory.Object)
                    .SetInformation(new MockClauseInformation(new ClauseNames("gin")));
                var clauseHandler2 = new MockClauseHandler()
                    .SetContextFactory(clauseContextFactory2.Object)
                    .SetInformation(new MockClauseInformation(new ClauseNames("gin")));

                var searchHandlerManager = new Mock<ISearchHandlerManager>();
                searchHandlerManager.Setup(x => x.GetClauseHandler((User) null, "gin"))
                    .Returns(new HashSet<IClauseHandler> {clauseHandler, clauseHandler2});

                var contextSetUtil = new Mock<ContextSetUtil>();
                contextSetUtil.Setup(x => x.Union(new HashSet<IClauseContext> {clauseContext1, clauseContext2})).Returns(clauseContext3);

                var visitorFactory = new QueryContextVisitor.QueryContextVisitorFactory(contextSetUtil.Object, searchHandlerManager.Object);
                var visitor = visitorFactory.CreateVisitor(null);
                var result = termClause1.Accept(visitor);

                var expectedResult = new QueryContextVisitor.ContextResult(clauseContext3, ClauseContext.CreateGlobalClauseContext());

                Assert.Equal(expectedResult, result);
            }

            [Fact]
            public void TestTerminalClause_Explicit()
            {
                var termClause1 = new TerminalClause("catalog", Operator.EQUALS, "1");

                var clauseContext1 = CreateCatalog(1);

                var clauseContextFactory = new Mock<IClauseContextFactory>();
                clauseContextFactory.Setup(x => x.GetClauseContext(null, termClause1)).Returns(clauseContext1);

                var clauseHandler = new MockClauseHandler()
                    .SetContextFactory(clauseContextFactory.Object)
                    .SetInformation(new MockClauseInformation(SystemSearchConstants.ForCatalog().JqlClauseNames));

                var searchHandlerManager = new Mock<ISearchHandlerManager>();
                searchHandlerManager.Setup(x => x.GetClauseHandler((User) null, "catalog"))
                    .Returns(new HashSet<IClauseHandler> {clauseHandler});

                var contextSetUtil = new Mock<ContextSetUtil>();

                var visitorFactory = new QueryContextVisitor.QueryContextVisitorFactory(contextSetUtil.Object, searchHandlerManager.Object);
                var visitor = visitorFactory.CreateVisitor(null);
                var result = termClause1.Accept(visitor);

                var expectedResult = new QueryContextVisitor.ContextResult(clauseContext1, clauseContext1);

                Assert.Equal(expectedResult, result);
            }

            [Fact]
            public void TestTerminalClause_EmptyContext()
            {
                var termClause1 = new TerminalClause("catalog", Operator.EQUALS, "1");

                var clauseContextFactory = new Mock<IClauseContextFactory>();
                clauseContextFactory.Setup(x => x.GetClauseContext(null, termClause1)).Returns(new ClauseContext());

                var clauseHandler = new MockClauseHandler()
                    .SetContextFactory(clauseContextFactory.Object)
                    .SetInformation(new MockClauseInformation(SystemSearchConstants.ForCatalog().JqlClauseNames));

                var searchHandlerManager = new Mock<ISearchHandlerManager>();
                searchHandlerManager.Setup(x => x.GetClauseHandler((User)null, "catalog"))
                    .Returns(new HashSet<IClauseHandler> { clauseHandler });

                var contextSetUtil = new Mock<ContextSetUtil>();

                var visitorFactory = new QueryContextVisitor.QueryContextVisitorFactory(contextSetUtil.Object, searchHandlerManager.Object);
                var visitor = visitorFactory.CreateVisitor(null);
                var result = termClause1.Accept(visitor);

                var expectedResult = new QueryContextVisitor.ContextResult(ClauseContext.CreateGlobalClauseContext(), ClauseContext.CreateGlobalClauseContext());

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