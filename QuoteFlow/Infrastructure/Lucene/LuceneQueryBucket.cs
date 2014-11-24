using System.Collections.Generic;
using Lucene.Net.Search;

namespace QuoteFlow.Infrastructure.Lucene
{
    /// <summary>
    /// Takes the BooleanClauses and puts them into MUST_NOT, MUST, and SHOULD buckets.
    /// </summary>
    public class LuceneQueryBucket
    {
        public virtual BooleanQuery OriginalBooleanQuery { get; private set; }
        public virtual IList<Query> NotQueries { get; private set; }
        public virtual IList<Query> ShouldQueries { get; private set; }

        internal readonly IList<Query> MustQueries;

        public LuceneQueryBucket(BooleanQuery booleanQuery)
        {
            OriginalBooleanQuery = booleanQuery;
            NotQueries = new List<Query>();
            MustQueries = new List<Query>();
            ShouldQueries = new List<Query>();
            Init(booleanQuery.Clauses);
        }

        public virtual bool ContainsOnlyNot()
        {
            return NotQueries.Count > 0 && MustQueries.Count == 0 && ShouldQueries.Count == 0;
        }

        public virtual bool ContainsMust()
        {
            return MustQueries.Count > 0;
        }

        public virtual bool ContainsShould()
        {
            return ShouldQueries.Count > 0;
        }

        protected virtual void Init(IEnumerable<BooleanClause> booleanClauses)
        {
            // Run through all the clauses and bucket them by the occurances we encounter
            foreach (BooleanClause booleanClause in booleanClauses)
            {
                var clauseOccur = booleanClause.Occur;
                Query clauseQuery = booleanClause.Query;

                if (Occur.MUST_NOT == clauseOccur)
                {
                    // We don't want to add these right away since we may be re-writing the single terms into BooleanQueries with a MatchAll
                    NotQueries.Add(clauseQuery);
                }
                else if (Occur.MUST == clauseOccur)
                {
                    MustQueries.Add(clauseQuery);
                }
                else if (Occur.SHOULD == clauseOccur)
                {
                    ShouldQueries.Add(clauseQuery);
                }
            }
        }
    }
}