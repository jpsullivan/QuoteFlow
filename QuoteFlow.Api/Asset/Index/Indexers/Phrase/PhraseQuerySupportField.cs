using Lucene.Net.Documents;

namespace QuoteFlow.Api.Asset.Index.Indexers.Phrase
{
    /// <summary>
    /// Encapsulates the information to create a special purpose <seealso cref="Field"/> to be used for quoted phrase 
    /// query searches for a given QuoteFlow text field.
    /// </summary>
    public class PhraseQuerySupportField
    {
        private const string PhraseQuerySupportFieldPrefix = "pq_support_";

        /// <summary>
        /// Returns the name of the phrase query support field to build for a given field.
        /// </summary>
        /// <param name="indexFieldName"> The name of the original field.
        /// </param>
        /// <returns> A {@code String} containing the name of the phrase query support field. </returns>
        public static string ForIndexField(string indexFieldName)
        {
            return PhraseQuerySupportFieldPrefix + indexFieldName;
        }

        /// <summary>
        /// Determines whether a given document field is a phrase query support field.
        /// </summary>
        /// <param name="indexFieldName"> the name of the field to inspect.
        /// </param>
        /// <returns> {@code true} if the passed in field is a phrase query support field; otherwise, {@code false}. </returns>
        public static bool IsPhraseQuerySupportField(string indexFieldName)
        {
            return indexFieldName.StartsWith(PhraseQuerySupportFieldPrefix);
        }
    }
}