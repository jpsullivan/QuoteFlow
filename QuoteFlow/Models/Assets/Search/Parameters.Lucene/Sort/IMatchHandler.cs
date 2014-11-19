using Lucene.Net.Index;

namespace QuoteFlow.Models.Assets.Search.Parameters.Lucene.Sort
{
    /// <summary>
    /// Allows <seealso cref="QuoteFlowLuceneFieldFinder"/> to handle matched terms in a customized way.
    /// </summary>
    public interface IMatchHandler
    {
        /// <summary>
        /// Invoked by <seealso cref="QuoteFlowLuceneFieldFinder.GetMatches(IndexReader, string, IMatchHandler)"/>
        /// for each field value for each document.  The calls will be made in
        /// order of increasing term values, with the document identifiers supplied
        /// in an arbitrary order.
        /// </summary>
        /// <param name="doc">The document identifier for the document that contains the term. In QuoteFlow, this indentifies a particular issue</param>
        /// <param name="termValue">The value assigned to the term. In QuoteFlow, this is the value assigned to the field.</param>
        void HandleMatchedDocument(int doc, string termValue);
    }
}