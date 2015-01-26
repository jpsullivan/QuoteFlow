using Lucene.Net.Search;

namespace QuoteFlow.Api.Infrastructure.Lucene
{
    /// <summary>
    /// This class will clone the <seealso cref="Query"/> and add a
    /// <seealso cref="MatchAllDocsQuery"/> to the portion of the query that require them.
    /// 
    /// This inspects the query to determine if there are any nodes in the query that are marked as
    /// <seealso cref="BooleanClause.Occur#MUST_NOT"/> AND they do not have a positive query to work
    /// against.
    /// 
    /// This is because Lucene will drop queries of this kind instead of trying to find the correct result.
    /// 
    /// When we specify a query that is -A || B lucene treats this as equivilent to B. When we specify this query what we
    /// mean is (-A && ALL_VALUES) || B which is obviously not equivilent to B.
    /// 
    /// The algorithm for determining if a <seealso cref="BooleanQuery"/> should have a
    /// <seealso cref="MatchAllDocsQuery"/> added to it with an occur of
    /// <seealso cref="BooleanClause.Occur#MUST_NOT"/> is:
    /// 
    /// Case 1: BooleanQuery contains only <seealso cref="BooleanClause.Occur#MUST_NOT"/> clauses THEN add a
    /// <seealso cref="MatchAllDocsQuery"/>
    /// 
    /// Case 2: BooleanQuery contains at least one <seealso cref="BooleanClause.Occur#MUST"/> and no
    /// <seealso cref="BooleanClause.Occur#SHOULD"/> THEN do not add a <seealso cref="MatchAllDocsQuery"/>
    /// 
    /// Case 3: BooleanQuery contains at least one <seealso cref="BooleanClause.Occur#SHOULD"/> THEN
    /// add a <seealso cref="MatchAllDocsQuery"/> to each
    /// <seealso cref="BooleanClause.Occur#MUST_NOT"/> portion of the query. This may mean that we need to
    /// rewrite the a single term to be a BooleanQuery that contains the single term AND the <seealso cref="MatchAllDocsQuery"/>.
    /// 
    /// NOTE: A BooleanQuery that contains at least one <seealso cref="BooleanClause.Occur#MUST"/> and at least
    /// one <seealso cref="BooleanClause.Occur#SHOULD"/> is the same as Case 2 since the MUST portion of the
    /// query will provide a positive set of results.
    /// </summary>
    public interface ILuceneQueryModifier
    {
        /// <summary>
        /// Will clone and rewrite the query as per the rules defined above.
        /// </summary>
        /// <param name="originalQuery"> defines the lucene query to inspect, must not be null. </param>
        /// <returns> the modified query that will return the right results when run. </returns>
        Query GetModifiedQuery(Query originalQuery);
    }
}