using System.Collections.Generic;
using Lucene.Net.Search;
using QuoteFlow.Api.Jql.Query.Clause;

namespace QuoteFlow.Api.Jql.Query
{
    /// <summary>
    /// Represents the result of a call to the <see cref="IClauseQueryFactory.GetQuery(IQueryCreationContext, ITerminalClause)"/>
    /// method. The result contains the a Lucene Query and a flag to indicate whether or not 
    /// the Lucene Query should be negated. When the flag is set to true QuoteFlow will automatically 
    /// negate the Lucene Query when it is run in Lucene.
    /// </summary>
    public class QueryFactoryResult
    {
        public global::Lucene.Net.Search.Query LuceneQuery { get; set; }
        public bool MustNotOccur { get; set; }

        private static readonly QueryFactoryResult FalseResult = new QueryFactoryResult(new BooleanQuery(), false);

        /// <summary>
        /// Create the result with the passed result and flag.
        /// </summary>
        /// <param name="luceneQuery">The query to add. Must not be null.</param>
        /// <param name="mustNotOccur">The flag to add to the result.</param>
        public QueryFactoryResult(global::Lucene.Net.Search.Query luceneQuery, bool mustNotOccur = false)
        {
            LuceneQuery = luceneQuery;
            MustNotOccur = mustNotOccur;
        }

        /// <summary>
        /// Creates a <see cref="QueryFactoryResult"/> instance that will return no results.
        /// </summary>
        /// <returns>A <see cref="QueryFactoryResult"/> instance that will return no results.</returns>
        public static QueryFactoryResult CreateFalseResult()
        {
            return FalseResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"> the field to be visible </param>
        /// <param name="result"> the result to wrap </param>
        /// <returns>
        /// A new <see cref="QueryFactoryResult"/> that combines the visibility query with
        /// the input result, and that always has mustNotOccur() == false
        /// </returns>
        public static QueryFactoryResult WrapWithVisibilityQuery(string fieldName, QueryFactoryResult result)
        {
            // don't bother wrapping a false result because it will return nothing anyway
            if (FalseResult.Equals(result))
            {
                return result;
            }

            var finalQuery = new BooleanQuery();
            AddToBooleanWithMust(result, finalQuery);
            finalQuery.Add(TermQueryFactory.VisibilityQuery(fieldName), Occur.MUST);
            return new QueryFactoryResult(finalQuery);
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="results">A list of results you want to merge; must not be null or contain nulls</param>
        /// <returns>
        /// non-false results merged in a new boolean query with SHOULD. The result should never need negation, i.e.
        /// <seealso cref="MustNotOccur"/> will always be false.
        /// </returns>
        public static QueryFactoryResult MergeResultsWithShould(IEnumerable<QueryFactoryResult> results)
        {
            var finalQuery = new BooleanQuery();
            foreach (QueryFactoryResult result in results)
            {
                if (!FalseResult.Equals(result))
                {
                    AddToBooleanWithShould(result, finalQuery);
                }
            }

            return new QueryFactoryResult(finalQuery);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="booleanQuery"></param>
        private static void AddToBooleanWithMust(QueryFactoryResult result, BooleanQuery booleanQuery)
        {
            AddToBooleanWithOccur(result, booleanQuery, Occur.MUST);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="booleanQuery"></param>
        private static void AddToBooleanWithShould(QueryFactoryResult result, BooleanQuery booleanQuery)
        {
            AddToBooleanWithOccur(result, booleanQuery, Occur.SHOULD);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="booleanQuery"></param>
        /// <param name="occur"></param>
        private static void AddToBooleanWithOccur(QueryFactoryResult result, BooleanQuery booleanQuery, Occur occur)
        {
            booleanQuery.Add(result.LuceneQuery, result.MustNotOccur ? Occur.MUST_NOT : occur);
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            QueryFactoryResult that = (QueryFactoryResult) obj;

            if (MustNotOccur != that.MustNotOccur)
            {
                return false;
            }
            if (!LuceneQuery.Equals(that.LuceneQuery))
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int result = LuceneQuery.GetHashCode();
            result = 31 * result + (MustNotOccur ? 1 : 0);
            return result;
        }

        public override string ToString()
        {
            return string.Format("QueryFactoryResult { LuceneQuery={0}, MustNotOccur={1} }", LuceneQuery, MustNotOccur);
        }
    }
}