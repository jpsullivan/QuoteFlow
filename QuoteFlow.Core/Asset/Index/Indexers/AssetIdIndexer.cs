using Lucene.Net.Documents;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Asset.Search.Constants;

namespace QuoteFlow.Core.Asset.Index.Indexers
{
    public class AssetIdIndexer : BaseFieldIndexer
    {
        public override string Id => SystemSearchConstants.ForAssetId().FieldId;

        public override string DocumentFieldId => SystemSearchConstants.ForAssetId().IndexField;

        public override bool IsFieldVisibleAndInScope(Api.Models.Asset asset)
        {
            return true;
        }

        public override void AddIndex(Document doc, Api.Models.Asset asset)
        {
            if ((asset == null) || asset.Id <= 0)
            {
                return;
            }

            var id = asset.Id;
            IndexKeyword(doc, DocumentConstants.AssetId, id.ToString(), asset);
            IndexFoldedKeyword(doc, DocumentFieldId, id.ToString(), asset);
        }
    }
}