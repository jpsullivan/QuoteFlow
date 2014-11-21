using System;
using Lucene.Net.Documents;
using QuoteFlow.Models.Assets.Search.Constants;

namespace QuoteFlow.Models.Assets.Index.Indexers
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

		public override bool IsFieldVisibleAndInScope(Asset asset)
		{
			return true;
		}

		public override void AddIndex(Document doc, Asset asset)
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