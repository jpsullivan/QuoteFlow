using Lucene.Net.Documents;

namespace QuoteFlow.Core.Asset.Index.Indexers
{
    /// <summary>
    /// Abstract FieldIndexer that has helper methods to index usernames in a case-insensitive manner.
    /// </summary>
    public abstract class UserFieldIndexer : BaseFieldIndexer
    {
        protected void IndexUserKey(Document doc, string indexField, string userkey, Api.Models.Asset asset)
        {
            IndexKeyword(doc, indexField, userkey, asset);
        }

        /// <summary>
        /// Index a single userkey field (case intact), with a default if the field is not set 
        /// </summary>
        protected void IndexUserkeyWithDefault(Document doc, string indexField, string userkey, string defaultValue, Api.Models.Asset asset)
        {
            IndexKeywordWithDefault(doc, indexField, userkey, defaultValue, asset);
        }
    }
}