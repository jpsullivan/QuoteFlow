using Lucene.Net.Documents;
using QuoteFlow.Api.Asset.Search.Constants;

namespace QuoteFlow.Core.Asset.Index.Indexers
{
    public class CreatedDateIndexer : BaseFieldIndexer
    {
        public override string Id
        {
            get { return SystemSearchConstants.ForCreatedDate().FieldId; }
        }

        public override string DocumentFieldId
        {
            get { return SystemSearchConstants.ForCreatedDate().IndexField; }
        }

        public override bool IsFieldVisibleAndInScope(Api.Models.Asset asset)
        {
            return true;
        }

        public override void AddIndex(Document doc, Api.Models.Asset asset)
        {
            IndexDateField(doc, DocumentFieldId, asset.CreationDate, asset);
        }
    }
}