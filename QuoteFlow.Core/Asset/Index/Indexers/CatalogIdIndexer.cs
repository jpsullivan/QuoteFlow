using System;
using Lucene.Net.Documents;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Asset.Search.Constants;

namespace QuoteFlow.Core.Asset.Index.Indexers
{
    public class CatalogIdIndexer : BaseFieldIndexer
    {
        public virtual string Id
		{
			get { return SystemSearchConstants.ForCatalog().FieldId; }
		}

		public virtual string DocumentFieldId
		{
			get { return SystemSearchConstants.ForCatalog().IndexField; }
		}

		public override bool IsFieldVisibleAndInScope(Api.Models.Asset asset)
		{
			return true;
		}

		public override void AddIndex(Document doc, Api.Models.Asset asset)
		{
            IndexKeyword(doc, DocumentFieldId, Convert.ToString(asset.Id), asset);
            // For sorting
		    string sku = asset.SKU;
            if (sku != null)
            {
                IndexKeyword(doc, DocumentConstants.CatalogId, sku, asset);
            }
		}
    }
}