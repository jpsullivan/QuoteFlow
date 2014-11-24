using Lucene.Net.Index;
using Lucene.Net.Search;

namespace QuoteFlow.Infrastructure.Lucene
{
    /// <summary>
    /// Creates <see cref="PrefixQuery"/> instances that doesn't rewrite into a <see cref="BooleanQuery"/> 
    /// with all matching <see cref="TermQuery"/> terms in the field. This query returns a constant score 
    /// equal to its boost for all documents with the matching prefix term.
    /// This can be significantly cheaper and faster if there are a lot of matching terms.
    /// It is very slightly slower if the number of matched terms is one or two.
    /// </summary>
    public class ConstantScorePrefixQuery
    {
        public static PrefixQuery Build(Term term)
        {
            var prefixQuery = new PrefixQuery(term)
            {
                RewriteMethod = PrefixQuery.CONSTANT_SCORE_FILTER_REWRITE
            };
            return prefixQuery;
        }
    }
}