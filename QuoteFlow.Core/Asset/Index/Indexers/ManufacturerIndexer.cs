using Lucene.Net.Documents;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Asset.Index.Indexers
{
    public class ManufacturerIndexer: BaseFieldIndexer
	{
        public override string Id => SystemSearchConstants.ForManufacturer().FieldId;

        public override string DocumentFieldId => SystemSearchConstants.ForManufacturer().IndexField;

        public override bool IsFieldVisibleAndInScope(Api.Models.Asset asset)
        {
            return true;
        }

		public override void AddIndex(Document doc, Api.Models.Asset asset)
		{
			Manufacturer manufacturer = asset.Manufacturer;
			if (manufacturer != null) // this should only be null in tests
			{
				IndexKeyword(doc, DocumentFieldId, manufacturer.Id.ToString(), asset);
			}
		}
	}
}