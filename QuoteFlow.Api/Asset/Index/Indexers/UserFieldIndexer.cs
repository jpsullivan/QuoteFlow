using Lucene.Net.Documents;

namespace QuoteFlow.Api.Asset.Index.Indexers
{
    /// <summary>
    /// Abstract FieldIndexer that has helper methods to index usernames in a case-insensitive manner.
    /// </summary>
    public abstract class UserFieldIndexer : BaseFieldIndexer
    {
        protected internal UserFieldIndexer()
        {
        }

        protected internal virtual void IndexUserKey(Document doc, string indexField, string userkey, Models.Asset asset)
        {
            IndexKeyword(doc, indexField, userkey, asset);
        }

        /// <summary>
        /// Index a single userkey field (case intact), with a default if the field is not set 
        /// </summary>
        protected internal virtual void IndexUserkeyWithDefault(Document doc, string indexField, string userkey, string defaultValue, Models.Asset asset)
        {
            IndexKeywordWithDefault(doc, indexField, userkey, defaultValue, asset);
        }
    }
}