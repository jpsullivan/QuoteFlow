using System.Collections.Generic;
using Lucene.Net.QueryParsers;
using Moq;
using QuoteFlow.Api.Asset.Index.Analyzer;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Lucene.Parsing;
using QuoteFlow.Api.Lucene.Index;
using QuoteFlow.Core.Jql.Query;
using QuoteFlow.Core.Jql.Query.Lucene.Parsing;
using QuoteFlow.Core.Tests.Jql.Query.Operand;
using Xunit;
using Xunit.Extensions;

namespace QuoteFlow.Core.Tests.Jql.Query
{
    public class LikeQueryFactoryTests
    {

        public class TheCreateQueryForSingleValueMethod
        {
            [Fact]
            public void NullRawValues()
            {
                var luceneQueryParserFactory = new Mock<ILuceneQueryParserFactory>();
                luceneQueryParserFactory
                    .Setup(x => x.CreateParserFor("testField"))
                    .Returns(new LuceneQueryParserFactory().CreateParserFor("testField"));

                var likeQueryFactory = new LikeQueryFactoryHelper(luceneQueryParserFactory.Object);

                var result = likeQueryFactory.CreateQueryForSingleValue("testField", Operator.LIKE, null);
                Assert.Equal(QueryFactoryResult.CreateFalseResult(), result);
            }

            [Fact]
            public void EmptyRawValues()
            {
                var luceneQueryParserFactory = new Mock<ILuceneQueryParserFactory>();
                luceneQueryParserFactory
                    .Setup(x => x.CreateParserFor("testField"))
                    .Returns(new LuceneQueryParserFactory().CreateParserFor("testField"));

                var likeQueryFactory = new LikeQueryFactoryHelper(luceneQueryParserFactory.Object);

                var result = likeQueryFactory.CreateQueryForSingleValue("testField", Operator.LIKE,
                    new List<QueryLiteral>());
                Assert.Equal(QueryFactoryResult.CreateFalseResult(), result);
            }

            [Fact]
            public void EmptyString()
            {
                var luceneQueryParserFactory = new Mock<ILuceneQueryParserFactory>();
                luceneQueryParserFactory
                    .Setup(x => x.CreateParserFor("testField"))
                    .Returns(new LuceneQueryParserFactory().CreateParserFor("testField"));

                var likeQueryFactory = new LikeQueryFactoryHelper(luceneQueryParserFactory.Object);

                var result = likeQueryFactory.CreateQueryForSingleValue("testField", Operator.LIKE,
                    new List<QueryLiteral> {SimpleLiteralFactory.CreateLiteral("")});
                Assert.Equal(QueryFactoryResult.CreateFalseResult(), result);
            }

            [Fact]
            public void BlankString()
            {
                var luceneQueryParserFactory = new Mock<ILuceneQueryParserFactory>();
                luceneQueryParserFactory
                    .Setup(x => x.CreateParserFor("testField"))
                    .Returns(new LuceneQueryParserFactory().CreateParserFor("testField"));

                var likeQueryFactory = new LikeQueryFactoryHelper(luceneQueryParserFactory.Object);

                var result = likeQueryFactory.CreateQueryForSingleValue("testField", Operator.LIKE,
                    new List<QueryLiteral> {SimpleLiteralFactory.CreateLiteral(" ")});
                Assert.Equal(QueryFactoryResult.CreateFalseResult(), result);
            }

            private class LikeQueryFactoryHelper : LikeQueryFactory
            {
                private readonly ILuceneQueryParserFactory _luceneQueryParserFactory;

                public LikeQueryFactoryHelper(ILuceneQueryParserFactory luceneQueryParserFactory)
                {
                    _luceneQueryParserFactory = luceneQueryParserFactory;
                }

                public override QueryParser GetQueryParser(string fieldName)
                {
                    return _luceneQueryParserFactory.CreateParserFor(fieldName);
                }
            }

            [Fact]
            public void HappyPathInt()
            {
                var likeQueryFactory = new HappyPathHelper();
                var result = likeQueryFactory.CreateQueryForSingleValue("test", Operator.LIKE,
                    new List<QueryLiteral> {SimpleLiteralFactory.CreateLiteral(42)});
                Assert.Equal("+test:42 +nonemptyfieldids:test +visiblefieldids:test", result.LuceneQuery.ToString());
            }

            [Fact]
            public void MultipleValuesPositive()
            {
                var likeQueryFactory = new HappyPathHelperNonMainIndex();
                var result = likeQueryFactory.CreateQueryForSingleValue("test", Operator.LIKE,
                    new List<QueryLiteral>
                    {
                        SimpleLiteralFactory.CreateLiteral(42),
                        SimpleLiteralFactory.CreateLiteral("blah")
                    });
                Assert.False(result.MustNotOccur);
                Assert.Equal("+(test:42 test:blah)", result.LuceneQuery.ToString());
            }

            [Fact]
            public void MultipleValuesNegative()
            {
                var likeQueryFactory = new HappyPathHelperNonMainIndex();
                var result = likeQueryFactory.CreateQueryForSingleValue("test", Operator.NOT_LIKE,
                    new List<QueryLiteral>
                    {
                        SimpleLiteralFactory.CreateLiteral(42),
                        SimpleLiteralFactory.CreateLiteral("blah")
                    });
                Assert.False(result.MustNotOccur);
                Assert.Equal("+(-test:42 -test:blah)", result.LuceneQuery.ToString());
            }

            [Fact]
            public void MultipleValuesPositive_WithEmpty()
            {
                var likeQueryFactory = new HappyPathHelperMainIndex();
                var result = likeQueryFactory.CreateQueryForSingleValue("test", Operator.LIKE,
                    new List<QueryLiteral> {SimpleLiteralFactory.CreateLiteral(42), new QueryLiteral()});
                Assert.False(result.MustNotOccur);
                Assert.Equal("+(test:42 (-nonemptyfieldids:test +visiblefieldids:test))", result.LuceneQuery.ToString());
            }

            [Fact]
            public void MultipleValuesNegative_WithEmpty()
            {
                var likeQueryFactory = new HappyPathHelperMainIndex();
                var result = likeQueryFactory.CreateQueryForSingleValue("test", Operator.NOT_LIKE,
                    new List<QueryLiteral> {SimpleLiteralFactory.CreateLiteral(42), new QueryLiteral()});
                Assert.False(result.MustNotOccur);
                Assert.Equal("+(-test:42 +nonemptyfieldids:test)", result.LuceneQuery.ToString());
            }

            [Fact]
            public void MultipleValuesPositive_WithEmptyButDoesntHandle()
            {
                var likeQueryFactory = new HappyPathHelperNonMainIndex();
                var result = likeQueryFactory.CreateQueryForSingleValue("test", Operator.LIKE,
                    new List<QueryLiteral> {SimpleLiteralFactory.CreateLiteral(42), new QueryLiteral()});
                Assert.False(result.MustNotOccur);
                Assert.Equal("+(test:42)", result.LuceneQuery.ToString());
            }

            [Fact]
            public void MultipleValuesNegative_WithEmptyButDoesntHandle()
            {
                var likeQueryFactory = new HappyPathHelperNonMainIndex();
                var result = likeQueryFactory.CreateQueryForSingleValue("test", Operator.NOT_LIKE,
                    new List<QueryLiteral> {SimpleLiteralFactory.CreateLiteral(42), new QueryLiteral()});
                Assert.False(result.MustNotOccur);
                Assert.Equal("+(-test:42)", result.LuceneQuery.ToString());
            }

            [Fact]
            public void HappyPathDoNotExcludeEmpties()
            {
                var likeQueryFactory = new HappyPathHelperNonMainIndex();
                var result = likeQueryFactory.CreateQueryForSingleValue("test", Operator.LIKE,
                    new List<QueryLiteral> {SimpleLiteralFactory.CreateLiteral(42)});
                Assert.False(result.MustNotOccur);
                Assert.Equal("+test:42", result.LuceneQuery.ToString());
            }

            [Fact]
            public void HappyPathToString()
            {
                var likeQueryFactory = new HappyPathHelper();
                var result = likeQueryFactory.CreateQueryForSingleValue("test", Operator.LIKE,
                    new List<QueryLiteral> {SimpleLiteralFactory.CreateLiteral("dude")});
                Assert.False(result.MustNotOccur);
                Assert.Equal("+test:dude +nonemptyfieldids:test +visiblefieldids:test", result.LuceneQuery.ToString());
            }

            [Fact]
            public void HappyPathComplexString()
            {
                var likeQueryFactory = new HappyPathHelper();
                var result = likeQueryFactory.CreateQueryForSingleValue("test", Operator.LIKE,
                    new List<QueryLiteral> {SimpleLiteralFactory.CreateLiteral("-dude +cool stuff")});
                Assert.False(result.MustNotOccur);
                Assert.Equal("+(-test:dude +test:cool +test:stuff) +nonemptyfieldids:test +visiblefieldids:test",
                    result.LuceneQuery.ToString());
            }

            [Fact]
            public void HappyPathComplexString_WithEscaping()
            {
                var likeQueryFactory = new HappyPathHelper();
                var result = likeQueryFactory.CreateQueryForSingleValue("test", Operator.LIKE,
                    new List<QueryLiteral> {SimpleLiteralFactory.CreateLiteral("-dude:1 +\"cool 2\" stuff\\3")});
                Assert.False(result.MustNotOccur);
                Assert.Equal(
                    "+(-test:\"dude 1\" +test:\"cool 2\" +test:\"stuff 3\") +nonemptyfieldids:test +visiblefieldids:test",
                    result.LuceneQuery.ToString());
            }

            [Fact]
            public void WithColon()
            {
                var likeQueryFactory = new HappyPathHelper();
                var result = likeQueryFactory.CreateQueryForSingleValue("test", Operator.LIKE,
                    new List<QueryLiteral> {SimpleLiteralFactory.CreateLiteral("-field:dude")});
                Assert.False(result.MustNotOccur);
                Assert.Equal("+(-test:\"field dude\") +nonemptyfieldids:test +visiblefieldids:test",
                    result.LuceneQuery.ToString());
            }

            [Fact]
            public void NotQueryHappyPathInt()
            {
                var likeQueryFactory = new HappyPathHelper();
                var result = likeQueryFactory.CreateQueryForSingleValue("test", Operator.NOT_LIKE,
                    new List<QueryLiteral> {SimpleLiteralFactory.CreateLiteral(42)});
                Assert.False(result.MustNotOccur);
                Assert.Equal("-test:42 +nonemptyfieldids:test +visiblefieldids:test", result.LuceneQuery.ToString());
            }

            [Fact]
            public void NotQueryHappyPathString()
            {
                var likeQueryFactory = new HappyPathHelper();
                var result = likeQueryFactory.CreateQueryForSingleValue("test", Operator.NOT_LIKE,
                    new List<QueryLiteral> {SimpleLiteralFactory.CreateLiteral("dude")});
                Assert.False(result.MustNotOccur);
                Assert.Equal("-test:dude +nonemptyfieldids:test +visiblefieldids:test", result.LuceneQuery.ToString());
            }

            [Fact]
            public void NotQueryHappyPathComplexString()
            {
                var likeQueryFactory = new HappyPathHelper();
                var result = likeQueryFactory.CreateQueryForSingleValue("test", Operator.NOT_LIKE,
                    new List<QueryLiteral> {SimpleLiteralFactory.CreateLiteral("-dude +cool stuff")});
                Assert.False(result.MustNotOccur);
                Assert.Equal("-(-test:dude +test:cool +test:stuff) +nonemptyfieldids:test +visiblefieldids:test",
                    result.LuceneQuery.ToString());
            }

            [Fact]
            public void NotQueryHappyPathComplexString_WithEscaping()
            {
                var likeQueryFactory = new HappyPathHelper();
                var result = likeQueryFactory.CreateQueryForSingleValue("test", Operator.NOT_LIKE,
                    new List<QueryLiteral> {SimpleLiteralFactory.CreateLiteral("-dude:1 +\"cool 2\" stuff\\3")});
                Assert.False(result.MustNotOccur);
                Assert.Equal(
                    "-(-test:\"dude 1\" +test:\"cool 2\" +test:\"stuff 3\") +nonemptyfieldids:test +visiblefieldids:test",
                    result.LuceneQuery.ToString());
            }

            [Fact]
            public void NotQueryWithColon()
            {
                var likeQueryFactory = new HappyPathHelper();
                var result = likeQueryFactory.CreateQueryForSingleValue("test", Operator.NOT_LIKE,
                    new List<QueryLiteral> {SimpleLiteralFactory.CreateLiteral("-field:dude")});
                Assert.False(result.MustNotOccur);
                Assert.Equal("-(-test:\"field dude\") +nonemptyfieldids:test +visiblefieldids:test",
                    result.LuceneQuery.ToString());
            }

            [Fact]
            public void InvalidQuery()
            {
                var likeQueryFactory = new HappyPathHelper();
                var result = likeQueryFactory.CreateQueryForSingleValue("test", Operator.LIKE,
                    new List<QueryLiteral> {SimpleLiteralFactory.CreateLiteral("^field:-dude")});
                Assert.Equal(QueryFactoryResult.CreateFalseResult(), result);
            }

            [Fact]
            public void InvalidQuery_BadFuzzy()
            {
                var likeQueryFactory = new HappyPathHelper();
                var result = likeQueryFactory.CreateQueryForSingleValue("test", Operator.LIKE,
                    new List<QueryLiteral> {SimpleLiteralFactory.CreateLiteral("dude~1")});
                Assert.Equal(QueryFactoryResult.CreateFalseResult(), result);
            }

            [Theory]
            [InlineData(Operator.GREATER_THAN_EQUALS)]
            [InlineData(Operator.GREATER_THAN)]
            [InlineData(Operator.LESS_THAN)]
            [InlineData(Operator.LESS_THAN_EQUALS)]
            [InlineData(Operator.IN)]
            [InlineData(Operator.NOT_IN)]
            [InlineData(Operator.IS)]
            [InlineData(Operator.IS_NOT)]
            [InlineData(Operator.EQUALS)]
            [InlineData(Operator.NOT_EQUALS)]
            public void ReturnsFalseResult_ForOperator(Operator op)
            {
                ReturnsFalseResultForOperator(op);
            }

            private void ReturnsFalseResultForOperator(Operator op)
            {
                var result = new LikeQueryFactory().CreateQueryForSingleValue("test", op, new List<QueryLiteral>());
                Assert.Equal(QueryFactoryResult.CreateFalseResult(), result);
            }

            [Fact]
            public void EmptyLiteralSameAs_EmptyQuery()
            {
                var likeQueryFactory = new HappyPathHelper();
                var forSingleValue = likeQueryFactory.CreateQueryForSingleValue("test", Operator.LIKE,
                    new List<QueryLiteral> {new QueryLiteral()});
                var forEmptyOperand = likeQueryFactory.CreateQueryForEmptyOperand("test", Operator.LIKE);

                Assert.Equal(forEmptyOperand, forSingleValue);
            }

            #region LikeQueryFactory Overrides

            private class HappyPathHelper : LikeQueryFactory
            {
                public override QueryParser GetQueryParser(string fieldName)
                {
                    return new QueryParser(LuceneVersion.Get(), fieldName,
                        new EnglishAnalyzer(LuceneVersion.Get(), false, TokenFilters.English.Stemming.Aggressive,
                            TokenFilters.English.StopWordRemoval.DefaultSet));
                }
            }

            private class HappyPathHelperMainIndex : LikeQueryFactory
            {
                public HappyPathHelperMainIndex()
                    : base(true)
                {
                }

                public override QueryParser GetQueryParser(string fieldName)
                {
                    return new QueryParser(LuceneVersion.Get(), fieldName,
                        new EnglishAnalyzer(LuceneVersion.Get(), false, TokenFilters.English.Stemming.Aggressive,
                            TokenFilters.English.StopWordRemoval.DefaultSet));
                }
            }

            private class HappyPathHelperNonMainIndex : LikeQueryFactory
            {
                public HappyPathHelperNonMainIndex()
                    : base(false)
                {
                }

                public override QueryParser GetQueryParser(string fieldName)
                {
                    return new QueryParser(LuceneVersion.Get(), fieldName,
                        new EnglishAnalyzer(LuceneVersion.Get(), false, TokenFilters.English.Stemming.Aggressive,
                            TokenFilters.English.StopWordRemoval.DefaultSet));
                }
            }

            #endregion
        }

        public class TheCreateQueryForMultipleValuesMethod
        {
            [Theory]
            [InlineData(Operator.LESS_THAN)]
            [InlineData(Operator.LESS_THAN_EQUALS)]
            [InlineData(Operator.GREATER_THAN)]
            [InlineData(Operator.GREATER_THAN_EQUALS)]
            [InlineData(Operator.EQUALS)]
            [InlineData(Operator.NOT_EQUALS)]
            [InlineData(Operator.IS)]
            [InlineData(Operator.IS_NOT)]
            [InlineData(Operator.IN)]
            [InlineData(Operator.NOT_IN)]
            [InlineData(Operator.LIKE)]
            [InlineData(Operator.NOT_LIKE)]
            public void ReturnsFalseResult_ForOperator(Operator op)
            {
                ReturnsFalseResultForOperator(op);
            }

            [Fact]
            public void MultipleValuesForLike()
            {
                var likeQueryFactory = new LikeQueryFactory();
                var result = likeQueryFactory.CreateQueryForMultipleValues("test", Operator.LIKE,
                    new List<QueryLiteral>());
                Assert.Equal(QueryFactoryResult.CreateFalseResult(), result);
            }

            private void ReturnsFalseResultForOperator(Operator op)
            {
                var result = new LikeQueryFactory().CreateQueryForMultipleValues("test", op, new List<QueryLiteral>());
                Assert.Equal(QueryFactoryResult.CreateFalseResult(), result);
            }
        }

        public class TheCreateQueryForEmptyOperandMethod
        {
            [Fact]
            public void ReturnsTheCorrectOutput()
            {
                var likeQueryFactory = new LikeQueryFactory();
                var emptyQuery = likeQueryFactory.CreateQueryForEmptyOperand("test", Operator.LIKE);
                Assert.False(emptyQuery.MustNotOccur);
                Assert.Equal("-nonemptyfieldids:test +visiblefieldids:test", emptyQuery.LuceneQuery.ToString());
                emptyQuery = likeQueryFactory.CreateQueryForEmptyOperand("test", Operator.IS);
                Assert.False(emptyQuery.MustNotOccur);
                Assert.Equal("-nonemptyfieldids:test +visiblefieldids:test", emptyQuery.LuceneQuery.ToString());
            }

            [Fact]
            public void NotEmptyOperators()
            {
                var likeQueryFactory = new LikeQueryFactory();
                var emptyQuery = likeQueryFactory.CreateQueryForEmptyOperand("test", Operator.NOT_LIKE);
                Assert.False(emptyQuery.MustNotOccur);
                Assert.Equal("nonemptyfieldids:test", emptyQuery.LuceneQuery.ToString());
                emptyQuery = likeQueryFactory.CreateQueryForEmptyOperand("test", Operator.IS_NOT);
                Assert.False(emptyQuery.MustNotOccur);
                Assert.Equal("nonemptyfieldids:test", emptyQuery.LuceneQuery.ToString());
            }

            [Theory]
            [InlineData(Operator.EQUALS)]
            [InlineData(Operator.NOT_EQUALS)]
            [InlineData(Operator.IN)]
            [InlineData(Operator.NOT_IN)]
            [InlineData(Operator.GREATER_THAN)]
            [InlineData(Operator.GREATER_THAN_EQUALS)]
            [InlineData(Operator.LESS_THAN)]
            [InlineData(Operator.LESS_THAN_EQUALS)]
            public void BadOperator(Operator op)
            {
                TestForBadOperator(op);
            }

            private void TestForBadOperator(Operator op)
            {
                var result = new LikeQueryFactory().CreateQueryForEmptyOperand("test", op);
                Assert.Equal(QueryFactoryResult.CreateFalseResult(), result);
            }
        }
    }

    public class TheHandlesOperatorMethod
    {
        public static IEnumerable<object[]> HandleList
        {
            get
            {
                yield return new object[] {Operator.GREATER_THAN, false};
                yield return new object[] {Operator.GREATER_THAN_EQUALS, false};
                yield return new object[] {Operator.LESS_THAN, false};
                yield return new object[] {Operator.LESS_THAN_EQUALS, false};
                yield return new object[] {Operator.EQUALS, false};
                yield return new object[] {Operator.NOT_EQUALS, false};
                yield return new object[] {Operator.IN, false};
                yield return new object[] {Operator.NOT_IN, false};
                yield return new object[] {Operator.LIKE, true};
                yield return new object[] {Operator.NOT_LIKE, true};
                yield return new object[] {Operator.IS, true};
                yield return new object[] {Operator.IS_NOT, true};
            }
        }

        [Theory]
        [PropertyData("HandleList")]
        public void ReturnsTheCorrectOutput(Operator op, bool result)
        {
            var likeQueryFactory = new LikeQueryFactory();
            Assert.Equal(result, likeQueryFactory.HandlesOperator(op));
        }
    }
}
