using Lucene.Net.Analysis;
using QuoteFlow.Api.Infrastructure.Lucene;
using Attribute = Lucene.Net.Util.Attribute;

namespace QuoteFlow.Infrastructure.Lucene
{
    /// <summary>
    /// This attribute can be used to mark a token as a keyword. Keyword aware
    /// <seealso cref="TokenStream"/>s can decide to modify a token based on the return value
    /// of <seealso cref="IsKeyword()"/> if the token is modified. Stemming filters for
    /// instance can use this attribute to conditionally skip a term if
    /// <seealso cref="IsKeyword()"/> returns <code>true</code>.
    /// </summary>
    public class KeywordAttribute : Attribute, IKeywordAttribute
    {
        public bool Keyword { get; set; }

        public override void Clear()
        {
            Keyword = false;
        }

        public override int GetHashCode()
        {
            return Keyword ? 31 : 37;
        }

        public override bool Equals(object other)
        {
            if (this == other)
            {
                return true;
            }
            if (this.GetType() != other.GetType())
            {
                return false;
            }
            var obj = (KeywordAttribute) other;
            return Keyword == obj.Keyword;

        }

        public override void CopyTo(Attribute target)
        {
            var attr = target as KeywordAttribute;
            if (attr != null) attr.Keyword = Keyword;
        }

        /// <summary>
        /// Returns <code>true</code> if the current token is a keyword, otherwise <code>false</code>.
        /// </summary>
        /// <returns></returns>
        public bool IsKeyword()
        {
            return Keyword;
        }
    }
}