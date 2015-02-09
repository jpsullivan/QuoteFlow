using System.Collections.Generic;
using Lucene.Net.Index;
using Lucene.Net.Search;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Jql.Query;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Query
{
    public class QueryFactoryResultTests
    {
        public class TheWrapWithVisibilityQueryMethod
        {
            [Fact]
            public void FalseResult()
            {
                var input = QueryFactoryResult.CreateFalseResult();
                var result = QueryFactoryResult.WrapWithVisibilityQuery("test", input);
                var expectedQuery = new BooleanQuery();

                Assert.Equal(expectedQuery, result.LuceneQuery);
                Assert.False(result.MustNotOccur);
            }

            [Fact]
            public void TermQuery()
            {
                var termQuery = new TermQuery(new Term("test", "123"));
                var input = new QueryFactoryResult(termQuery);
                var result = QueryFactoryResult.WrapWithVisibilityQuery("test", input);
                var expectedQuery = new BooleanQuery();
                expectedQuery.Add(termQuery, Occur.MUST);
                expectedQuery.Add(Vis(), Occur.MUST);

                Assert.Equal(expectedQuery, result.LuceneQuery);
                Assert.False(result.MustNotOccur);
            }

            [Fact]
            public void TermQuery_MustNotOccur()
            {
                var termQuery = new TermQuery(new Term("test", "123"));
                var input = new QueryFactoryResult(termQuery, true);
                var result = QueryFactoryResult.WrapWithVisibilityQuery("test", input);
                var expectedQuery = new BooleanQuery();
                expectedQuery.Add(termQuery, Occur.MUST_NOT);
                expectedQuery.Add(Vis(), Occur.MUST);

                Assert.Equal(expectedQuery, result.LuceneQuery);
                Assert.False(result.MustNotOccur);
            }

            private static TermQuery Vis()
            {
                return new TermQuery(new Term(DocumentConstants.AssetVisibleFieldIds, "test"));
            }
        }

        public class TheMergeResultsWithShouldMethod
        {
            [Fact]
            public void EmptyList()
            {
                Assert.Equal(QueryFactoryResult.CreateFalseResult(), QueryFactoryResult.MergeResultsWithShould(new List<QueryFactoryResult>()));
            }

            [Fact]
            public void AllFalse()
            {
                var list = new List<QueryFactoryResult>()
                {
                    QueryFactoryResult.CreateFalseResult(),
                    QueryFactoryResult.CreateFalseResult()
                };
                
                Assert.Equal(QueryFactoryResult.CreateFalseResult(), QueryFactoryResult.MergeResultsWithShould(list));
            }

            [Fact]
            public void OneFalse_TwoNot()
            {
                var query1 = TermQueryFactory.NonEmptyQuery("field1");
                var query2 = TermQueryFactory.NonEmptyQuery("field2");
                var result1 = new QueryFactoryResult(query1, false);
                var result2 = new QueryFactoryResult(query2, true);
                var list = new List<QueryFactoryResult> {QueryFactoryResult.CreateFalseResult(), result1, result2};

                var expectedQuery = new BooleanQuery();
                expectedQuery.Add(query1, Occur.SHOULD);
                expectedQuery.Add(query2, Occur.MUST_NOT);
                var expectedResult = new QueryFactoryResult(expectedQuery);

                Assert.Equal(expectedResult, QueryFactoryResult.MergeResultsWithShould(list));
            }
        }
    }
}
