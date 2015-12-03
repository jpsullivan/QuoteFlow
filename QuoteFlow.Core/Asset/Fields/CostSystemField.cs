using QuoteFlow.Api.Asset;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Handlers;
using QuoteFlow.Api.Asset.Statistics;
using QuoteFlow.Core.Asset.Search.Handlers;

namespace QuoteFlow.Core.Asset.Fields
{
    public class CostSystemField : NavigableField, ISearchableField
    {
        private readonly ISearchHandlerFactory _searchHandlerFactory;

        public CostSystemField(CostSearchHandlerFactory handlerFactory)
            : base(AssetFieldConstants.Cost, "asset.field.cost", "asset.column.heading.cost", NavigableFieldOrder.Descending)
        {
            _searchHandlerFactory = handlerFactory;
        }

        public new ILuceneFieldSorter<int?> Sorter => IntegerFieldStatisticsMapper.Cost;

        public SearchHandler CreateAssociatedSearchHandler()
        {
            return _searchHandlerFactory.CreateHandler(this);
        }
    }
}