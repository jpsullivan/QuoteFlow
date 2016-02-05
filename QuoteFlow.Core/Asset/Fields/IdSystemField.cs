using System.Collections.Generic;
using Lucene.Net.Search;
using QuoteFlow.Api.Asset;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Parameters.Lucene.Sort;

namespace QuoteFlow.Core.Asset.Fields
{
    public class IdSystemField : NavigableField
    {
        public IdSystemField()
            : base(AssetFieldConstants.Id, "asset.field.assetId", "asset.column.heading.assetId", NavigableFieldOrder.Descending)
        {
        }

        public override IEnumerable<SortField> GetSortFields(bool sortOrder)
        {
            return new List<SortField>
            {
                new SortField(DocumentConstants.CatalogId, new StringSortComparator(), sortOrder)
            };
        }
    }
}