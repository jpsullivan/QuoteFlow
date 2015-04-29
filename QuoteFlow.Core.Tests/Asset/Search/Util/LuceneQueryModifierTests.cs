using Lucene.Net.Index;
using Lucene.Net.Search;
using QuoteFlow.Core.Asset.Search.Util;
using Xunit;

namespace QuoteFlow.Core.Tests.Asset.Search.Util
{
    public class LuceneQueryModifierTests
    {
        [Fact]
        public void TestNotChanged_If_NoBooleanQuery()
        {
            var blahQuery = GetTermQuery("blah", "blee");
            Assert.Equal(blahQuery, new LuceneQueryModifier().GetModifiedQuery(blahQuery));
        }

        [Fact]
        public void TestPassedEmptyBooleanQuery()
        {
            Assert.Equal(new BooleanQuery(), new LuceneQueryModifier().GetModifiedQuery(new BooleanQuery()));
        }

        [Fact]
        public void TestContainsOnlyNotQueries()
        {
            var queryToTransform = new BooleanQuery();
            var blahQuery = GetTermQuery("blah", "blee");
            blahQuery.Boost = 9;
            queryToTransform.Add(blahQuery, Occur.MUST_NOT);
            var clahQuery = GetTermQuery("clah", "clee");
            queryToTransform.Add(clahQuery, Occur.MUST_NOT);
            queryToTransform.Boost = 3;
            queryToTransform.MinimumNumberShouldMatch = 4;

            var expectedQuery = new BooleanQuery
            {
                Boost = 3,
                MinimumNumberShouldMatch = 4
            };
            expectedQuery.Add(new MatchAllDocsQuery(), Occur.MUST);
            expectedQuery.Add(blahQuery, Occur.MUST_NOT);
            expectedQuery.Add(clahQuery, Occur.MUST_NOT);

            Assert.Equal(expectedQuery, new LuceneQueryModifier().GetModifiedQuery(queryToTransform));
        }

        [Fact]
        public void TestContainsOnlyNotWithNestedNotQueries()
        {
            BooleanQuery queryToTransform = new BooleanQuery();

            TermQuery blahQuery = GetTermQuery("blah", "blee");
            queryToTransform.Add(blahQuery, Occur.MUST_NOT);

            BooleanQuery nestedNotQuery = new BooleanQuery
            {
                MinimumNumberShouldMatch = 3,
                Boost = 4
            };
            TermQuery clahQuery = GetTermQuery("clah", "clee");
            nestedNotQuery.Add(clahQuery, Occur.MUST_NOT);
            TermQuery dlahQuery = GetTermQuery("dlah", "dlee");
            nestedNotQuery.Add(dlahQuery, Occur.MUST_NOT);

            queryToTransform.Add(nestedNotQuery, Occur.MUST_NOT);

            BooleanQuery expectedQuery = new BooleanQuery {{new MatchAllDocsQuery(), Occur.MUST}};
            BooleanQuery expectedNestedNotQuery = new BooleanQuery
            {
                MinimumNumberShouldMatch = 3,
                Boost = 4
            };
            expectedNestedNotQuery.Add(new MatchAllDocsQuery(), Occur.MUST);
            expectedNestedNotQuery.Add(clahQuery, Occur.MUST_NOT);
            expectedNestedNotQuery.Add(dlahQuery, Occur.MUST_NOT);
            expectedQuery.Add(blahQuery, Occur.MUST_NOT);
            expectedQuery.Add(expectedNestedNotQuery, Occur.MUST_NOT);

            Assert.Equal(expectedQuery, (new LuceneQueryModifier()).GetModifiedQuery(queryToTransform));
        }

        [Fact]
        public void TestContainsMustWithNotQueries()
        {
            BooleanQuery queryToTransform = new BooleanQuery();
            TermQuery blahQuery = GetTermQuery("blah", "blee");
            queryToTransform.Add(blahQuery, Occur.MUST_NOT);
            TermQuery clahQuery = GetTermQuery("clah", "clee");
            queryToTransform.Add(clahQuery, Occur.MUST_NOT);
            TermQuery dlahQuery = GetTermQuery("dlah", "dlee");
            queryToTransform.Add(dlahQuery, Occur.MUST);

            BooleanQuery expectedQuery = new BooleanQuery
            {
                {blahQuery, Occur.MUST_NOT},
                {clahQuery, Occur.MUST_NOT},
                {dlahQuery, Occur.MUST}
            };

            Assert.Equal(expectedQuery, (new LuceneQueryModifier()).GetModifiedQuery(queryToTransform));
        }

        [Fact]
        public void TestContainsMustWithNotAndShouldQueries()
        {
            BooleanQuery queryToTransform = new BooleanQuery();
            TermQuery blahQuery = GetTermQuery("blah", "blee");
            queryToTransform.Add(blahQuery, Occur.MUST_NOT);
            TermQuery clahQuery = GetTermQuery("clah", "clee");
            queryToTransform.Add(clahQuery, Occur.MUST_NOT);
            TermQuery dlahQuery = GetTermQuery("dlah", "dlee");
            queryToTransform.Add(dlahQuery, Occur.MUST);
            TermQuery elahQuery = GetTermQuery("elah", "elee");
            queryToTransform.Add(elahQuery, Occur.SHOULD);

            BooleanQuery expectedQuery = new BooleanQuery
            {
                {blahQuery, Occur.MUST_NOT},
                {clahQuery, Occur.MUST_NOT},
                {dlahQuery, Occur.MUST},
                {elahQuery, Occur.SHOULD}
            };

            Assert.Equal(expectedQuery, (new LuceneQueryModifier()).GetModifiedQuery(queryToTransform));
        }

        [Fact]
        public void TestContainsMustWithNestedNotQueries()
        {
            BooleanQuery queryToTransform = new BooleanQuery();
			BooleanQuery nestedNotQuery = new BooleanQuery();
			TermQuery blahQuery = GetTermQuery("blah", "blee");
			nestedNotQuery.Add(blahQuery, Occur.MUST_NOT);
			TermQuery clahQuery = GetTermQuery("clah", "clee");
			nestedNotQuery.Add(clahQuery, Occur.MUST_NOT);
			TermQuery dlahQuery = GetTermQuery("dlah", "dlee");
			queryToTransform.Add(nestedNotQuery, Occur.MUST);
			queryToTransform.Add(dlahQuery, Occur.MUST);

			BooleanQuery expectedQuery = new BooleanQuery();
            BooleanQuery expectedNestedNotQuery = new BooleanQuery
            {
                {new MatchAllDocsQuery(), Occur.MUST},
                {blahQuery, Occur.MUST_NOT},
                {clahQuery, Occur.MUST_NOT}
            };
            expectedQuery.Add(expectedNestedNotQuery, Occur.MUST);
			expectedQuery.Add(dlahQuery, Occur.MUST);

            Assert.Equal(expectedQuery, (new LuceneQueryModifier()).GetModifiedQuery(queryToTransform));
        }

        [Fact]
        public void TestContainsMustWithBooleanNestedNotQueries()
        {
            BooleanQuery queryToTransform = new BooleanQuery();
            BooleanQuery nestedNotQuery = new BooleanQuery();
            TermQuery blahQuery = GetTermQuery("blah", "blee");
            nestedNotQuery.Add(blahQuery, Occur.MUST);
            TermQuery clahQuery = GetTermQuery("clah", "clee");
            nestedNotQuery.Add(clahQuery, Occur.MUST);
            TermQuery dlahQuery = GetTermQuery("dlah", "dlee");
            queryToTransform.Add(nestedNotQuery, Occur.MUST_NOT);
            queryToTransform.Add(dlahQuery, Occur.MUST);

            BooleanQuery expectedQuery = new BooleanQuery
            {
                {nestedNotQuery, Occur.MUST_NOT}, 
                {dlahQuery, Occur.MUST}
            };

            Assert.Equal(queryToTransform, (new LuceneQueryModifier()).GetModifiedQuery(queryToTransform));
        }

        [Fact]
        public void TestContainsShouldWithNotQueries()
        {
            BooleanQuery queryToTransform = new BooleanQuery();
            TermQuery blahQuery = GetTermQuery("blah", "blee");
            queryToTransform.Add(blahQuery, Occur.MUST_NOT);
            TermQuery clahQuery = GetTermQuery("clah", "clee");
            clahQuery.Boost = 8;
            queryToTransform.Add(clahQuery, Occur.MUST_NOT);
            TermQuery dlahQuery = GetTermQuery("dlah", "dlee");
            queryToTransform.Add(dlahQuery, Occur.SHOULD);

            BooleanQuery expectedQuery = new BooleanQuery {{dlahQuery, Occur.SHOULD}};
            BooleanQuery subNotQuery1 = new BooleanQuery
            {
                {new MatchAllDocsQuery(), Occur.MUST},
                {blahQuery, Occur.MUST_NOT}
            };
            expectedQuery.Add(subNotQuery1, Occur.SHOULD);
            BooleanQuery subNotQuery2 = new BooleanQuery {Boost = 8};
            subNotQuery2.Add(new MatchAllDocsQuery(), Occur.MUST);
            TermQuery clahQueryWithNoBoost = GetTermQuery("clah", "clee");
            subNotQuery2.Add(clahQueryWithNoBoost, Occur.MUST_NOT);
            expectedQuery.Add(subNotQuery2, Occur.SHOULD);

            Assert.Equal(expectedQuery, (new LuceneQueryModifier()).GetModifiedQuery(queryToTransform));
        }

        [Fact]
        public void TestContainsShouldWithNestedNotQueries()
        {
            BooleanQuery queryToTransform = new BooleanQuery();
            BooleanQuery nestedNotQuery = new BooleanQuery();
            TermQuery blahQuery = GetTermQuery("blah", "blee");
            nestedNotQuery.Add(blahQuery, Occur.MUST_NOT);
            TermQuery clahQuery = GetTermQuery("clah", "clee");
            nestedNotQuery.Add(clahQuery, Occur.MUST_NOT);
            TermQuery dlahQuery = GetTermQuery("dlah", "dlee");
            queryToTransform.Add(nestedNotQuery, Occur.SHOULD);
            queryToTransform.Add(dlahQuery, Occur.SHOULD);

            BooleanQuery expectedQuery = new BooleanQuery();
            BooleanQuery expectedNestedNotQuery = new BooleanQuery();
            expectedNestedNotQuery.Add(new MatchAllDocsQuery(), Occur.MUST);
            expectedNestedNotQuery.Add(blahQuery, Occur.MUST_NOT);
            expectedNestedNotQuery.Add(clahQuery, Occur.MUST_NOT);
            expectedQuery.Add(expectedNestedNotQuery, Occur.SHOULD);
            expectedQuery.Add(dlahQuery, Occur.SHOULD);

            Assert.Equal(expectedQuery, (new LuceneQueryModifier()).GetModifiedQuery(queryToTransform));
        }

        [Fact]
        public void TestContainsShouldWithBooleanNestedNotQueries()
        {
            BooleanQuery queryToTransform = new BooleanQuery();
            BooleanQuery nestedNotQuery = new BooleanQuery();
            TermQuery blahQuery = GetTermQuery("blah", "blee");
            nestedNotQuery.Add(blahQuery, Occur.MUST);
            TermQuery clahQuery = GetTermQuery("clah", "clee");
            nestedNotQuery.Add(clahQuery, Occur.MUST);
            TermQuery dlahQuery = GetTermQuery("dlah", "dlee");
            queryToTransform.Add(nestedNotQuery, Occur.MUST_NOT);
            queryToTransform.Add(dlahQuery, Occur.SHOULD);

            BooleanQuery expectedQuery = new BooleanQuery {{dlahQuery, Occur.SHOULD}};
            BooleanQuery subExpectedNestedNotQuery = new BooleanQuery
            {
                {blahQuery, Occur.MUST}, 
                {clahQuery, Occur.MUST}
            };
            BooleanQuery expectedNestedNotQuery = new BooleanQuery
            {
                {new MatchAllDocsQuery(), Occur.MUST},
                {subExpectedNestedNotQuery, Occur.MUST_NOT}
            };

            expectedQuery.Add(expectedNestedNotQuery, Occur.SHOULD);

            Assert.Equal(expectedQuery, (new LuceneQueryModifier()).GetModifiedQuery(queryToTransform));
        }

        private TermQuery GetTermQuery(string fieldName, string indexValue)
        {
            return new TermQuery(new Term(fieldName, indexValue));
        }
    }
}