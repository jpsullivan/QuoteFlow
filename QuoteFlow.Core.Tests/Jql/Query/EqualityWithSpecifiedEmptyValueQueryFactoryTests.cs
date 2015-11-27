using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Core.Jql.Query;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Query
{
    public class EqualityWithSpecifiedEmptyValueQueryFactoryTests
    {
        [Fact]
        public void TestCreateQueryForEmptyOperandPositiveOperator()
        {
            var equalityQueryFactory = new EqualityWithSpecifiedEmptyValueQueryFactory<object>(null, "BLAH");
            var emptyQuery = equalityQueryFactory.CreateQueryForEmptyOperand("test", Operator.EQUALS);
            Assert.Equal("test:BLAH", emptyQuery.LuceneQuery.ToString());
            Assert.False(emptyQuery.MustNotOccur);

            emptyQuery = equalityQueryFactory.CreateQueryForEmptyOperand("test", Operator.IS);
            Assert.False(emptyQuery.MustNotOccur);
            Assert.Equal("test:BLAH", emptyQuery.LuceneQuery.ToString());
        }

        [Fact]
        public void TestCreateForEmptyOperandNegativeOperator()
        {
            var equalityQueryFactory = new EqualityWithSpecifiedEmptyValueQueryFactory<object>(null, "BLAH");
            QueryFactoryResult emptyQuery = equalityQueryFactory.CreateQueryForEmptyOperand("test", Operator.NOT_EQUALS);
            Assert.Equal("-test:BLAH +visiblefieldids:test", emptyQuery.LuceneQuery.ToString());
            Assert.False(emptyQuery.MustNotOccur);
            emptyQuery = equalityQueryFactory.CreateQueryForEmptyOperand("test", Operator.IS_NOT);
            Assert.False(emptyQuery.MustNotOccur);
            Assert.Equal("-test:BLAH +visiblefieldids:test", emptyQuery.LuceneQuery.ToString());
        }

        [Fact]
        public void TestCreateQueryForEmptyOperandBadOperators()
        {
            _testCreateQueryForEmptyOperandBadOperator(Operator.GREATER_THAN);
            _testCreateQueryForEmptyOperandBadOperator(Operator.GREATER_THAN_EQUALS);
            _testCreateQueryForEmptyOperandBadOperator(Operator.LESS_THAN);
            _testCreateQueryForEmptyOperandBadOperator(Operator.LESS_THAN_EQUALS);
            _testCreateQueryForEmptyOperandBadOperator(Operator.IN);
            _testCreateQueryForEmptyOperandBadOperator(Operator.NOT_IN);
            _testCreateQueryForEmptyOperandBadOperator(Operator.LIKE);
            _testCreateQueryForEmptyOperandBadOperator(Operator.NOT_LIKE);
        }

        private static void _testCreateQueryForEmptyOperandBadOperator(Operator oprator)
        {
            var queryFactory = new EqualityWithSpecifiedEmptyValueQueryFactory<object>(null, "BLAH");
            queryFactory.CreateQueryForEmptyOperand("test", oprator);
        }
    }
}