using System;
using Lucene.Net.Search;
using QuoteFlow.Api.Infrastructure.Lucene;

namespace QuoteFlow.Infrastructure.Lucene
{
    public class LuceneQueryModifier : ILuceneQueryModifier
    {
        public Query GetModifiedQuery(Query originalQuery)
        {
            if (originalQuery == null)
            {
                throw new ArgumentNullException("originalQuery");
            }

            if (originalQuery is BooleanQuery)
            {
                return TransformBooleanQuery((BooleanQuery) originalQuery);
            }

            return originalQuery;
        }

        private BooleanQuery TransformBooleanQuery(BooleanQuery originalQuery)
        {
            // See what kind of clauses our current BooleanQuery has
            var queryBucket = new LuceneQueryBucket(originalQuery);

            if (queryBucket.ContainsOnlyNot())
            {
                // Case 1
                return HandleOnlyNot(queryBucket);
            }
            if (queryBucket.ContainsMust())
            {
                // Case 2
                return HandleContainsMust(queryBucket);
            }
            if (queryBucket.ContainsShould())
            {
                // Case 3
                return HandleContainsShould(queryBucket);
            }

            // The query was empty
            return new BooleanQuery();
        }

        // Handles Case 3 as described in the interface
        private BooleanQuery HandleContainsShould(LuceneQueryBucket queryBucket)
        {
            var query = new BooleanQuery();
            BooleanQuery originalQuery = queryBucket.OriginalBooleanQuery;
            query.Boost = originalQuery.Boost;
            query.MinimumNumberShouldMatch = originalQuery.MinimumNumberShouldMatch;

            // Add all the positive SHOULD queries making sure to complete a deep dive on the BooleanQueries
            foreach (Query shouldQuery in queryBucket.ShouldQueries)
            {
                if (shouldQuery is BooleanQuery)
                {
                    query.Add(TransformBooleanQuery((BooleanQuery) shouldQuery), Occur.SHOULD);
                }
                else
                {
                    query.Add((Query) shouldQuery.Clone(), Occur.SHOULD);
                }
            }

            // Handle all the MUST_NOT queries expanding them as needed
            foreach (Query origNotQuery in queryBucket.NotQueries)
            {
                // We need to expand this to a BooleanQuery that contains a MatchAll
                var notWithMatchAll = new BooleanQuery();
                var notQuery = (Query) origNotQuery.Clone();
                notWithMatchAll.Boost = notQuery.Boost;
                notWithMatchAll.Add(new MatchAllDocsQuery(), Occur.MUST);
                // Reset this clauses boost since we have moved the boost onto the new parent BooleanClause
                notQuery.Boost = 1;

                if (notQuery is BooleanQuery)
                {
                    notWithMatchAll.MinimumNumberShouldMatch = ((BooleanQuery) notQuery).MinimumNumberShouldMatch;
                    // Reset this on the current query since the new parent BooleanClause is taking over its value
                    ((BooleanQuery) notQuery).MinimumNumberShouldMatch = 0;
                    notWithMatchAll.Add(TransformBooleanQuery((BooleanQuery) notQuery), Occur.MUST_NOT);
                }
                else
                {
                    notWithMatchAll.Add(notQuery, Occur.MUST_NOT);
                }
                query.Add(notWithMatchAll, Occur.SHOULD);
            }

            return query;
        }

        // Handles Case 2 as described in the interface
        private BooleanQuery HandleContainsMust(LuceneQueryBucket queryBucket)
        {
            var query = new BooleanQuery();

            // There is nothing to do here except complete a deep dive on each query
            var originalBooleanQuery = queryBucket.OriginalBooleanQuery;
            query.Boost = originalBooleanQuery.Boost;
            query.MinimumNumberShouldMatch = originalBooleanQuery.MinimumNumberShouldMatch;
            
            var booleanClauses = originalBooleanQuery.Clauses;
            foreach (BooleanClause booleanClause in booleanClauses)
            {
                Query subQuery = booleanClause.Query;
                var subOccur = booleanClause.Occur;
                if (subQuery is BooleanQuery)
                {
                    query.Add(TransformBooleanQuery((BooleanQuery) subQuery), subOccur);
                }
                else
                {
                    query.Add((Query) subQuery.Clone(), subOccur);
                }
            }
            return query;
        }

        // Handles Case 1 as described in the interface
        private BooleanQuery HandleOnlyNot(LuceneQueryBucket queryBucket)
        {
            var query = new BooleanQuery();

            BooleanQuery originalQuery = queryBucket.OriginalBooleanQuery;
            query.Boost = originalQuery.Boost;
            query.MinimumNumberShouldMatch = originalQuery.MinimumNumberShouldMatch;
            // We always add a match all in this case
            query.Add(new MatchAllDocsQuery(), Occur.MUST);

            foreach (Query origNotQuery in queryBucket.NotQueries)
            {
                var notQuery = (Query)origNotQuery.Clone();
                // Make sure we continue to dive the tree if we need to
                if (notQuery is BooleanQuery)
                {
                    query.Add(TransformBooleanQuery((BooleanQuery) notQuery), Occur.MUST_NOT);
                }
                else
                {
                    query.Add(notQuery, Occur.MUST_NOT);
                }
            }
            return query;
        }
    }
}