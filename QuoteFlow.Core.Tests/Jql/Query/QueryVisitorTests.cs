using System.Collections.Generic;
using System.Linq;
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

            var mockClause = new Mock<IClause>();
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
    }
}
