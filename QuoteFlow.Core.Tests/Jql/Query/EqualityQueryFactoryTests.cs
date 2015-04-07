using Lucene.Net.Index;
using Lucene.Net.Search;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Jql.Query;
using Xunit;
using Xunit.Extensions;

namespace QuoteFlow.Core.Tests.Jql.Query
{
    public class EqualityQueryFactoryTests
    {
        public class TheCreateQueryForEmptyOperandMethod
        {
            [Fact]
            public void EmptyOperand()
            {
                var equalityQueryFactory = new EqualityQueryFactory<object>(null);
                var emptyQuery = equalityQueryFactory.CreateQueryForEmptyOperand("test", Operator.EQUALS);
                Assert.Equal("-nonemptyfieldids:test +visiblefieldids:test", emptyQuery.LuceneQuery.ToString());
                Assert.False(emptyQuery.MustNotOccur);

                emptyQuery = equalityQueryFactory.CreateQueryForEmptyOperand("test", Operator.IS);
                Assert.False(emptyQuery.MustNotOccur);
                Assert.Equal("-nonemptyfieldids:test +visiblefieldids:test", emptyQuery.LuceneQuery.ToString());
            }

            [Fact]
            public void NotEmptyOperand()
            {
                var equalityQueryFactory = new EqualityQueryFactory<object>(null);
                var emptyQuery = equalityQueryFactory.CreateQueryForEmptyOperand("test", Operator.NOT_EQUALS);
                Assert.Equal("nonemptyfieldids:test", emptyQuery.LuceneQuery.ToString());
                Assert.False(emptyQuery.MustNotOccur);

                emptyQuery = equalityQueryFactory.CreateQueryForEmptyOperand("test", Operator.IS_NOT);
                Assert.False(emptyQuery.MustNotOccur);
                Assert.Equal("nonemptyfieldids:test", emptyQuery.LuceneQuery.ToString());
            }

            [Theory]
            [InlineData(Operator.LIKE)]
            [InlineData(Operator.NOT_LIKE)]
            [InlineData(Operator.IN)]
            [InlineData(Operator.NOT_IN)]
            [InlineData(Operator.GREATER_THAN)]
            [InlineData(Operator.GREATER_THAN_EQUALS)]
            [InlineData(Operator.LESS_THAN)]
            [InlineData(Operator.LESS_THAN_EQUALS)]
            public void CreateQueryForEmptyOperandBadOperator(Operator op)
            {
                var equalityQueryFactory = new EqualityQueryFactory<object>(null);
                var result = equalityQueryFactory.CreateQueryForEmptyOperand("test", op);
                Assert.Equal(QueryFactoryResult.CreateFalseResult(), result);
            }
        }

        public class TheGetIsNotEmptyQueryMethod
        {
            [Fact]
            public void ProperlyRetrievesNonEmptyQuery()
            {
                var equalityQueryFactory = new EqualityQueryFactory<object>(null);
                var notEmptyQuery = equalityQueryFactory.GetIsNotEmptyQuery("hello");
                Assert.Equal(new TermQuery(new Term(DocumentConstants.AssetNonEmptyFieldIds, "hello")), notEmptyQuery);
            }
        }
    }
}