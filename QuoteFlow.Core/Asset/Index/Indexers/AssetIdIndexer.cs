using Lucene.Net.Documents;
using QuoteFlow.Api.Asset.Search.Constants;

namespace QuoteFlow.Core.Asset.Index.Indexers
{
    public class AssetIdIndexer : BaseFieldIndexer
    {
        public string Id
        {
            get { return SystemSearchConstants.ForAssetId().FieldId; }
        }

        public string DocumentFieldId
        {
            get { return SystemSearchConstants.ForAssetId().IndexField; }
        }

        public override bool IsFieldVisibleAndInScope(Api.Models.Asset asset)
        {
            return true;
        }

        public override void AddIndex(Document doc, Api.Models.Asset asset)
        {
            IndexKeyword(doc, DocumentFieldId, asset.Id.ToString(), asset);
        }
    }
}