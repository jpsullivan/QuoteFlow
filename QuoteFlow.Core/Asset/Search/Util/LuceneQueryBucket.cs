using System.Collections.Generic;
using Lucene.Net.Search;

namespace QuoteFlow.Core.Asset.Search.Util
{
    /// <summary>
    /// Takes the BooleanClauses and puts them into MUST_NOT, MUST, and SHOULD buckets.
    /// </summary>
    public sealed class LuceneQueryBucket
    {
        public BooleanQuery OriginalBooleanQuery { get; private set; }
        public IList<Query> NotQueries { get; private set; }
        public IList<Query> ShouldQueries { get; private set; }

        private readonly IList<Query> _mustQueries;

        public LuceneQueryBucket(BooleanQuery booleanQuery)
        {
            OriginalBooleanQuery = booleanQuery;
            NotQueries = new List<Query>();
            _mustQueries = new List<Query>();
            ShouldQueries = new List<Query>();
            Init(booleanQuery.Clauses);
        }

        public bool ContainsOnlyNot()
        {
            return NotQueries.Count > 0 && _mustQueries.Count == 0 && ShouldQueries.Count == 0;
        }

        public bool ContainsMust()
        {
            return _mustQueries.Count > 0;
        }

        public bool ContainsShould()
        {
            return ShouldQueries.Count > 0;
        }

        private void Init(IEnumerable<BooleanClause> booleanClauses)
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
                    _mustQueries.Add(clauseQuery);
                }
                else if (Occur.SHOULD == clauseOccur)
                {
                    ShouldQueries.Add(clauseQuery);
                }
            }
        }
    }
}