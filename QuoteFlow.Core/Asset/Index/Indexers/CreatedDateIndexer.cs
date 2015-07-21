using Lucene.Net.Documents;
using QuoteFlow.Api.Asset.Search.Constants;

namespace QuoteFlow.Core.Asset.Index.Indexers
{
    public class CreatedDateIndexer : BaseFieldIndexer
    {
        public override string Id => SystemSearchConstants.ForCreatedDate().FieldId;

        public override string DocumentFieldId => SystemSearchConstants.ForCreatedDate().IndexField;

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