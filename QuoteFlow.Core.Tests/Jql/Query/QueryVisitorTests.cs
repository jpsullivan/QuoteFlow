using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Moq;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Core.Jql.Clause;
using QuoteFlow.Core.Jql.Query;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Query
{
    public class QueryVisitorTests
    {
        [Fact]
        public void VisitAndClause_MustNotOccur()
        {
            var deMorgansVisitor = new DeMorgansVisitor();
            var queryVisitor = new QueryVisitor(null, null, deMorgansVisitor, null, null);

            var mockClause = new Mock<IClause>(MockBehavior.Strict);
            mockClause.Setup(x => x.Accept(deMorgansVisitor)).Returns(mockClause.Object);
            mockClause.Setup(x => x.Accept(queryVisitor)).Returns(new QueryFactoryResult(new BooleanQuery(), true));

            var mockClause2 = new Mock<IClause>(MockBehavior.Strict);
            mockClause2.Setup(x => x.Accept(deMorgansVisitor)).Returns(mockClause.Object);
            mockClause2.Setup(x => x.Accept(queryVisitor)).Returns(new QueryFactoryResult(new BooleanQuery(), true));

            var andClause = new AndClause(mockClause.Object, mockClause2.Object);

            var query = queryVisitor.Visit(andClause);

            Assert.False(query.MustNotOccur);
            Assert.IsAssignableFrom<BooleanQuery>(query.LuceneQuery);

            var booleanQuery = query.LuceneQuery as BooleanQuery;
            List<BooleanClause> booleanClauses = booleanQuery.Clauses;
            Assert.Equal(2, booleanClauses.Count);
            Assert.Equal(Occur.MUST_NOT, booleanClauses.ElementAt(0).Occur);
            Assert.Equal(Occur.MUST_NOT, booleanClauses.ElementAt(1).Occur);

            mockClause.Verify();
            mockClause2.Verify();
        }

        [Fact]
        public void VisitAndClause_HappyPath()
        {
            var deMorgansVisitor = new DeMorgansVisitor();
            var queryVisitor = new QueryVisitor(null, null, deMorgansVisitor, null, null);

            var mockClause = new Mock<IClause>(MockBehavior.Strict);
            mockClause.Setup(x => x.Accept(deMorgansVisitor)).Returns(mockClause.Object);
            mockClause.Setup(x => x.Accept(queryVisitor)).Returns(new QueryFactoryResult(new BooleanQuery(), true));

            var mockClause2 = new Mock<IClause>(MockBehavior.Strict);
            mockClause2.Setup(x => x.Accept(deMorgansVisitor)).Returns(mockClause2.Object);
            mockClause2.Setup(x => x.Accept(queryVisitor)).Returns(QueryFactoryResult.CreateFalseResult);

            var andClause = new AndClause(mockClause.Object, mockClause2.Object);

            QueryFactoryResult query = queryVisitor.Visit(andClause);

            Assert.False(query.MustNotOccur);
            Assert.IsAssignableFrom<BooleanQuery>(query.LuceneQuery);
            
            var booleanQuery = query.LuceneQuery as BooleanQuery;
            List<BooleanClause> booleanClauses = booleanQuery.Clauses;
            Assert.Equal(2, booleanClauses.Count);
            Assert.Equal(Occur.MUST_NOT, booleanClauses.ElementAt(0).Occur);
            Assert.Equal(Occur.MUST, booleanClauses.ElementAt(1).Occur);

            mockClause.Verify();
            mockClause2.Verify();
        }

        [Fact]
        public void VisitOrClause_ContainsMustNotOccur()
        {
            DeMorgansVisitor deMorgansVisitor = new DeMorgansVisitor();
            QueryVisitor queryVisitor = new QueryVisitor(null, null, deMorgansVisitor, null, null);

            var mockClause = new Mock<IClause>(MockBehavior.Strict);
            mockClause.Setup(x => x.Accept(deMorgansVisitor)).Returns(mockClause.Object);
            mockClause.Setup(x => x.Accept(queryVisitor)).Returns(new QueryFactoryResult(new BooleanQuery(), true));

            var mockClause2 = new Mock<IClause>(MockBehavior.Strict);
            mockClause2.Setup(x => x.Accept(deMorgansVisitor)).Returns(mockClause2.Object);
            mockClause2.Setup(x => x.Accept(queryVisitor)).Returns(QueryFactoryResult.CreateFalseResult);

            var orClause = new OrClause(mockClause.Object, mockClause2.Object);

            QueryFactoryResult query = queryVisitor.Visit(orClause);

            Assert.False(query.MustNotOccur);
            Assert.IsAssignableFrom<BooleanQuery>(query.LuceneQuery);

            BooleanQuery booleanQuery = (BooleanQuery) query.LuceneQuery;
            var booleanClauses = booleanQuery.Clauses;
            Assert.Equal(2, booleanClauses.Count);
            Assert.Equal(Occur.SHOULD, booleanClauses.ElementAt(0).Occur);

            BooleanQuery expectedQuery = new BooleanQuery();
            expectedQuery.Add(new BooleanQuery(), Occur.MUST_NOT);
            Assert.Equal(expectedQuery, booleanClauses.ElementAt(0).Query);
            Assert.Equal(Occur.SHOULD, booleanClauses.ElementAt(1).Occur);
            Assert.Equal(new BooleanQuery(), booleanClauses[1].Query);

            mockClause.Verify();
            mockClause2.Verify();
        }

        [Fact]
        public void VisitOrClause_HappyPath()
        {
            DeMorgansVisitor deMorgansVisitor = new DeMorgansVisitor();
            QueryVisitor queryVisitor = new QueryVisitor(null, null, deMorgansVisitor, null, null);

            var mockClause = new Mock<IClause>(MockBehavior.Strict);
            mockClause.Setup(x => x.Accept(deMorgansVisitor)).Returns(mockClause.Object);
            mockClause.Setup(x => x.Accept(queryVisitor)).Returns(QueryFactoryResult.CreateFalseResult);

            var mockClause2 = new Mock<IClause>(MockBehavior.Strict);
            mockClause2.Setup(x => x.Accept(deMorgansVisitor)).Returns(mockClause2.Object);
            mockClause2.Setup(x => x.Accept(queryVisitor)).Returns(QueryFactoryResult.CreateFalseResult);
            
            var orClause = new OrClause(mockClause.Object, mockClause2.Object);

            QueryFactoryResult query = queryVisitor.Visit(orClause);

            Assert.False(query.MustNotOccur);
            Assert.IsAssignableFrom<BooleanQuery>(query.LuceneQuery);

            BooleanQuery booleanQuery = (BooleanQuery)query.LuceneQuery;
            var booleanClauses = booleanQuery.Clauses;
            Assert.Equal(2, booleanClauses.Count);
            Assert.Equal(Occur.SHOULD, booleanClauses.ElementAt(0).Occur);
            Assert.Equal(Occur.SHOULD, booleanClauses.ElementAt(1).Occur);

            mockClause.Verify();
            mockClause2.Verify();
        }

        [Fact]
        public void VisitTerminalClause_HappyPath()
        {
            DeMorgansVisitor deMorgansVisitor = new DeMorgansVisitor();
            var terminalClause = new TerminalClause("blah", Operator.EQUALS, "blee");

            var returnQuery = new BooleanQuery();

            var mockClauseQueryFactory = new Mock<IClauseQueryFactory>(MockBehavior.Strict);
            mockClauseQueryFactory.Setup(x => x.GetQuery(null, terminalClause)).Returns(new QueryFactoryResult(returnQuery));

            var mockQueryRegistry = new Mock<IQueryRegistry>(MockBehavior.Strict);
            mockQueryRegistry.Setup(x => x.GetClauseQueryFactory(null, terminalClause))
                .Returns(new List<IClauseQueryFactory> { mockClauseQueryFactory.Object });

            var queryVisitor = new QueryVisitor(mockQueryRegistry.Object, null, deMorgansVisitor, null, null);

            QueryFactoryResult query = queryVisitor.Visit(terminalClause);

            Assert.False(query.MustNotOccur);
            Assert.Equal(returnQuery, query.LuceneQuery);

            mockClauseQueryFactory.Verify();
            mockQueryRegistry.Verify();
        }

        [Fact]
        public void VisitTerminalClause_MultipleFactories_HappyPath()
        {
            DeMorgansVisitor deMorgansVisitor = new DeMorgansVisitor();
            TerminalClause terminalClause = new TerminalClause("blah", Operator.EQUALS, "blee");

            var returnQuery = new BooleanQuery();
            BooleanQuery returnQuery2 = new BooleanQuery();
            returnQuery2.Add(new TermQuery(new Term("bad", "query")), Occur.MUST);

            var mockClauseQueryFactory = new Mock<IClauseQueryFactory>(MockBehavior.Strict);
            mockClauseQueryFactory.Setup(x => x.GetQuery(null, terminalClause)).Returns(new QueryFactoryResult(returnQuery));

            var mockClauseQueryFactory2 = new Mock<IClauseQueryFactory>(MockBehavior.Strict);
            mockClauseQueryFactory2.Setup(x => x.GetQuery(null, terminalClause))
                .Returns(new QueryFactoryResult(returnQuery2, true));

            var mockQueryRegistry = new Mock<IQueryRegistry>(MockBehavior.Strict);
            mockQueryRegistry.Setup(x => x.GetClauseQueryFactory(null, terminalClause))
                .Returns(new List<IClauseQueryFactory> { mockClauseQueryFactory.Object, mockClauseQueryFactory2.Object });
            

            var queryVisitor = new QueryVisitor(mockQueryRegistry.Object, null, deMorgansVisitor, null, null);

            QueryFactoryResult query = queryVisitor.Visit(terminalClause);

            // The expected query.
            BooleanQuery expectedSubQuery = new BooleanQuery();
            expectedSubQuery.Add(returnQuery2, Occur.MUST_NOT);

            BooleanQuery expectedQuery = new BooleanQuery();
            expectedQuery.Add(returnQuery, Occur.SHOULD);
            expectedQuery.Add(expectedSubQuery, Occur.SHOULD);

            Assert.False(query.MustNotOccur);
            Assert.Equal(expectedQuery, query.LuceneQuery);

            mockClauseQueryFactory.Verify();
            mockQueryRegistry.Verify();
        }

        [Fact]
        public void VisitNotClause_HappyPath()
        {
            DeMorgansVisitor deMorgansVisitor = new DeMorgansVisitor();
            QueryVisitor queryVisitor = new QueryVisitor(null, null, deMorgansVisitor, null, null);

            NotClause badNot = new NotClause(new TerminalClause("blah", Operator.EQUALS, "blee"));
            var mockNotClause = new Mock<NotClause>(MockBehavior.Strict);
            mockNotClause.Setup(x => x.Accept(deMorgansVisitor)).Returns(badNot);

            var ex = Assert.Throws<InvalidOperationException>(() => queryVisitor.Visit(mockNotClause.Object));

            Assert.Equal("We have removed all the NOT clauses from the query, this should never occur.", ex.Message);

            mockNotClause.Verify();
        }

        [Fact]
        public void VisitTerminalClause_NoQueryFactory()
        {
            DeMorgansVisitor deMorgansVisitor = new DeMorgansVisitor();
            var terminalClause = new TerminalClause("blah", Operator.EQUALS, "blee");

            var mockQueryRegistry = new Mock<IQueryRegistry>(MockBehavior.Strict);
            mockQueryRegistry.Setup(x => x.GetClauseQueryFactory(null, terminalClause))
                .Returns(new List<IClauseQueryFactory>());

            var queryVisitor = new QueryVisitor(mockQueryRegistry.Object, null, deMorgansVisitor, null, null);

            QueryFactoryResult queryFactoryResult = queryVisitor.Visit(terminalClause);

            Assert.False(queryFactoryResult.MustNotOccur);
            Assert.Equal(new BooleanQuery(), queryFactoryResult.LuceneQuery);

            mockQueryRegistry.Verify();
        }

        public class TheCreateQueryMethod
        {
            // Make sure the query returned from this visit will not be ngeated when not necessary.
            [Fact]
            public void NotNegated()
            {
                DeMorgansVisitor deMorgansVisitor = new DeMorgansVisitor();
                QueryVisitor queryVisitor = new QueryVisitor(null, null, deMorgansVisitor, null, null);
                BooleanQuery expectedQuery = new BooleanQuery();

                var mockNotClause = new Mock<NotClause>();
                mockNotClause.Setup(x => x.Accept(deMorgansVisitor)).Returns(mockNotClause.Object);
                mockNotClause.Setup(x => x.Accept(queryVisitor)).Returns(new QueryFactoryResult(expectedQuery));


                var actualQuery = queryVisitor.CreateQuery(mockNotClause.Object);
                Assert.Equal(expectedQuery, actualQuery);

                mockNotClause.Verify();
            }

            // Make sure the Query returned from the visit will be negated if the result of the visit says
            // that it should be.
            [Fact]
            public void Negated()
            {
                var deMorgansVisitor = new DeMorgansVisitor();
                var queryVisitor = new QueryVisitor(null, null, deMorgansVisitor, null, null);
                var notQuery = new TermQuery(new Term("qwerty", "dddd"));
                var expectedQuery = new BooleanQuery();
                expectedQuery.Add(notQuery, Occur.MUST_NOT);

                var mockNotClause = new Mock<NotClause>();
                mockNotClause.Setup(x => x.Accept(deMorgansVisitor)).Returns(mockNotClause.Object);
                mockNotClause.Setup(x => x.Accept(queryVisitor)).Returns(new QueryFactoryResult(notQuery, true));


                var actualQuery = queryVisitor.CreateQuery(mockNotClause.Object);
                Assert.Equal(expectedQuery, actualQuery);

                mockNotClause.Verify();
            }

            [Fact]
            public void CreatedOnNormalizedClause()
            {
                var deMorgansVisitor = new DeMorgansVisitor();
                var queryVisitor = new QueryVisitor(null, null, deMorgansVisitor, null, null);
                var expectedQuery = new BooleanQuery();

                var mockNormalizedClause = new Mock<IClause>();
                mockNormalizedClause.Setup(x => x.Accept(queryVisitor)).Returns(new QueryFactoryResult(expectedQuery));

                var mockNotClause = new Mock<NotClause>();
                mockNotClause.Setup(x => x.Accept(deMorgansVisitor)).Returns(mockNormalizedClause.Object);

                var actualQuery = queryVisitor.CreateQuery(mockNotClause.Object);
                Assert.Equal(expectedQuery, actualQuery);

                mockNotClause.Verify();
                mockNormalizedClause.Verify();
            }
        }
    }
}
