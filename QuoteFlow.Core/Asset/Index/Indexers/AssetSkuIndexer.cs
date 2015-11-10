using Lucene.Net.Documents;
using QuoteFlow.Api.Asset.Index.Indexers.Phrase;
using QuoteFlow.Api.Asset.Search.Constants;

namespace QuoteFlow.Core.Asset.Index.Indexers
{
    public class AssetSkuIndexer : BaseFieldIndexer
    {
        public override string Id => SystemSearchConstants.ForSku().FieldId;

        public override string DocumentFieldId => SystemSearchConstants.ForSku().IndexField;

        public override bool IsFieldVisibleAndInScope(Api.Models.Asset asset)
        {
            return true;
        }

        public override void AddIndex(Document doc, Api.Models.Asset asset)
        {
            IndexText(doc, DocumentFieldId, asset.SKU, asset);
            IndexText(doc, PhraseQuerySupportField.ForIndexField(DocumentFieldId), asset.SKU, asset);
        }
    }
}