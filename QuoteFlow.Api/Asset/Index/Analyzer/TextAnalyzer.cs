using System.IO;
using Lucene.Net.Analysis;

namespace QuoteFlow.Api.Asset.Index.Analyzer
{
    /// <summary>
    /// Analyzereld.Index.Analyzer"/> base class that provides the facility to apply the <seealso cref="SubtokenFilter"/>
    /// during indexing and duplicating the original tokens before any stemming filter is applied to support wildcard
    /// queries and exact phrase queries on document fields.
    /// </summary>
    public abstract class TextAnalyzer : global::Lucene.Net.Analysis.Analyzer
    {
        public TextAnalyzer(bool indexing)
        {
            Indexing = indexing;
        }

        public bool Indexing { get; private set; }

        /// <summary>
        /// Applies a <seealso cref="SubtokenFilter"/> to the input token stream at document indexing time.
        /// </summary>
        /// <param name="input"> token stream </param>
        /// <returns> A TokenStream filtered by the sub-token filter during indexing, otherwise the input token stream is
        /// returned. </returns>
        protected TokenStream WrapStreamForIndexing(TokenStream input)
        {
            if (Indexing)
            {
                return new SubtokenFilter(input);
            }
            return input;
        }

        /// <summary>
        /// Applies a <seealso cref="KeywordRepeatFilter"/> to the input token stream at document indexing time to store the original
        /// tokens as keywords before any stemming filter is applied and therefore support wildcard searches and exact phrase
        /// queries on document fields.
        /// </summary>
        /// <param name="input"> token stream </param>
        /// <returns> A TokenStream filtered by the sub-token filter during indexing, otherwise the input token stream is
        /// returned. </returns>
        protected TokenStream WrapStreamForWilcardSearchSupport(TokenStream input)
        {
            // todo: lucene 4.8 upgrade
//            if (Indexing)
//            {
//                return new KeywordRepeatFilter(input);
//            }
            return input;
        }

        public override TokenStream ReusableTokenStream(string fieldName, TextReader reader)
        {
            return base.ReusableTokenStream(fieldName, reader);
        }
    }
}