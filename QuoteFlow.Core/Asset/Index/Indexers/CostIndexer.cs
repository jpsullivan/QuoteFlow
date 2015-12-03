using System.Globalization;
using Lucene.Net.Documents;
using QuoteFlow.Api.Asset.Search.Constants;

namespace QuoteFlow.Core.Asset.Index.Indexers
{
    public class CostIndexer : BaseFieldIndexer
    {
        public override string Id => SystemSearchConstants.ForCost().FieldId;

        public override string DocumentFieldId => SystemSearchConstants.ForCost().IndexField;

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

            // add actual vs estimated cost to index for range query - only add
            doc.Add(new Field(DocumentFieldId, asset.Cost.ToString(CultureInfo.InvariantCulture), Field.Store.YES,
                Field.Index.NOT_ANALYZED_NO_NORMS));
        }
    }
}