using QuoteFlow.Api.Jql.Query.Clause;

namespace QuoteFlow.Api.Jql.Query
{
    /// <summary>
    /// Converts a JQL query into an lucene query for searching QuoteFlow lucene index.
    /// 
    /// This should be used over using the QueryVisitor directly
    /// </summary>
    public interface ILuceneQueryBuilder
    {
        /// <summary>
        /// Converts a JQL <see cref="IClause"/> into an lucene <see cref="Query"/> for searching QuoteFlow lucene index.</summary>
        /// <param name="queryCreationContext">The security context under which the lucene query should be generated.</param>
        /// <param name="clause">The JQL clause to convert into a lucene query.</param>
        /// <returns>The lucene query generated from the <see cref="IClause"/>, Never null.</returns>
        global::Lucene.Net.Search.Query CreateLuceneQuery(IQueryCreationContext queryCreationContext, IClause clause);
    }
}