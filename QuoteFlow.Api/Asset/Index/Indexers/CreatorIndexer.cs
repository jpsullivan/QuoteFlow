using Lucene.Net.Documents;
using QuoteFlow.Api.Asset.Search.Constants;

namespace QuoteFlow.Api.Asset.Index.Indexers
{
    /// <summary>
    /// Class used for indexing the CreatorSystemField.
    /// </summary>
    public class CreatorIndexer : UserFieldIndexer
    {
        public virtual string Id
        {
            get { return SystemSearchConstants.ForCreator().FieldId; }
        }

        public virtual string DocumentFieldId
        {
            get { return SystemSearchConstants.ForCreator().IndexField; }
        }

        public override void AddIndex(Document doc, Models.Asset asset)
        {
            IndexUserkeyWithDefault(doc, DocumentFieldId, asset.CreatorId.ToString(), SystemSearchConstants.ForCreator().EmptyIndexValue, asset);
        }
    }
}