using Lucene.Net.Search;
using Moq;
using QuoteFlow.Api.Infrastructure.Lucene;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Core.Jql.Query;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Query
{
    public class LuceneQueryBuilderTests
    {
        public class TheCreateLuceneQueryMethod
        {
            private readonly Mock<IQueryRegistry> _queryRegistry;
            private readonly Mock<ILuceneQueryModifier> _luceneQueryModifier;

            public TheCreateLuceneQueryMethod()
            {
                _queryRegistry = new Mock<IQueryRegistry>();
                _luceneQueryModifier = new Mock<ILuceneQueryModifier>();
            }

            [Fact]
            public void TestHappyPath()
            {
                var clause = new TerminalClause("blah", Operator.EQUALS, "blah");
                var context = new QueryCreationContext(null);
                var falseResult = QueryFactoryResult.CreateFalseResult();

                QueryVisitor queryVisitor = new MyQueryVisitor(_queryRegistry.Object, context, falseResult, null, null);
                _luceneQueryModifier.Setup(x => x.GetModifiedQuery(falseResult.LuceneQuery)).Returns(falseResult.LuceneQuery);

                var mockBuilder = new Mock<LuceneQueryBuilder>(_queryRegistry.Object, _luceneQueryModifier.Object, null, null) { CallBase = true };
                mockBuilder.Setup(x => x.CreateQueryVisitor(context)).Returns(queryVisitor);

                var builder = mockBuilder.Object;

                Assert.Equal(falseResult.LuceneQuery, builder.CreateLuceneQuery(context, clause));
            }

            [Fact]
            public void TestVistorThrowsUp()
            {
                var clause = new TerminalClause("blah", Operator.EQUALS, "blah");
                var context = new QueryCreationContext(null);

                QueryVisitor queryVisitor = new MyQueryVisitor(_queryRegistry.Object, context, null, null, null);

                var mockBuilder = new Mock<LuceneQueryBuilder>(_queryRegistry.Object, _luceneQueryModifier.Object, null, null) { CallBase = true };
                mockBuilder.Setup(x => x.CreateQueryVisitor(context)).Returns(queryVisitor);

                var builder = mockBuilder.Object;

                Assert.Throws<ClauseTooComplexSearchException>(() => builder.CreateLuceneQuery(context, clause));
            }

            [Fact]
            public void TestModifierThrowsUp()
            {
                var clause = new TerminalClause("blah", Operator.EQUALS, "blah");
                var context = new QueryCreationContext(null);
                var falseResult = QueryFactoryResult.CreateFalseResult();

                QueryVisitor queryVisitor = new MyQueryVisitor(_queryRegistry.Object, context, falseResult, null, null);
                _luceneQueryModifier.Setup(x => x.GetModifiedQuery(falseResult.LuceneQuery))
                    .Throws<BooleanQuery.TooManyClauses>();

                var mockBuilder = new Mock<LuceneQueryBuilder>(_queryRegistry.Object, _luceneQueryModifier.Object, null, null) { CallBase = true };
                mockBuilder.Setup(x => x.CreateQueryVisitor(context)).Returns(queryVisitor);

                var builder = mockBuilder.Object;

                Assert.Throws<ClauseTooComplexSearchException>(() => builder.CreateLuceneQuery(context, clause));
            }
        }

        private class MyQueryVisitor : QueryVisitor
        {
            private readonly QueryFactoryResult _queryFactoryResult;

            internal MyQueryVisitor(IQueryRegistry queryRegistry, IQueryCreationContext queryCreationContext, QueryFactoryResult queryFactoryResult, WasClauseQueryFactory wasClauseQueryFactory, ChangedClauseQueryFactory changedClauseQueryFactory)
                : base(queryRegistry, queryCreationContext, wasClauseQueryFactory, changedClauseQueryFactory)
            {
                _queryFactoryResult = queryFactoryResult;
            }

            public override QueryFactoryResult Visit(ITerminalClause terminalClause)
            {
                if (_queryFactoryResult != null)
                {
                    return _queryFactoryResult;
                }
                
                throw new JqlTooComplex(terminalClause);
            }
        }

    }
}