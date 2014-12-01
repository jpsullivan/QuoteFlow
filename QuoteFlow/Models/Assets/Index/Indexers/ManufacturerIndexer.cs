using Lucene.Net.Documents;
using QuoteFlow.Models.Assets.Search.Constants;

namespace QuoteFlow.Models.Assets.Index.Indexers
{
    public class ManufacturerIndexer: BaseFieldIndexer
	{
        public virtual string Id
		{
			get { return SystemSearchConstants.ForManufacturer().FieldId; }
		}

		public virtual string DocumentFieldId
		{
			get { return SystemSearchConstants.ForManufacturer().IndexField; }
		}

        public override bool IsFieldVisibleAndInScope(Asset asset)
        {
            return true;
        }

		public override void AddIndex(Document doc, Asset asset)
		{
			Manufacturer manufacturer = asset.Manufacturer;
			if (manufacturer != null) // this should only be null in tests
			{
				IndexKeyword(doc, DocumentFieldId, manufacturer.Id.ToString(), asset);
			}
		}
	}
}