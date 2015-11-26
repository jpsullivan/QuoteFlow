using QuoteFlow.Api.Asset;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Handlers;
using QuoteFlow.Api.Asset.Statistics;
using QuoteFlow.Api.Models;
using QuoteFlow.Core.Asset.Search.Handlers;

namespace QuoteFlow.Core.Asset.Fields
{
    public class CreatorSystemField : NavigableField, ISearchableField
    {
        private readonly ISearchHandlerFactory _searchHandlerFactory;
        private readonly CreatorStatisticsMapper _creatorStatisticsMapper;

        public CreatorSystemField(CreatorSearchHandlerFactory searchHandlerFactory, CreatorStatisticsMapper statisticsMapper) 
            : base(AssetFieldConstants.Creator, "asset.field.creator", "asset.column.heading.creator", NavigableFieldOrder.Descending)
        {
            _searchHandlerFactory = searchHandlerFactory;
            _creatorStatisticsMapper = statisticsMapper;
        }

        public new ILuceneFieldSorter<User> Sorter => _creatorStatisticsMapper;

        public SearchHandler CreateAssociatedSearchHandler()
        {
            return _searchHandlerFactory.CreateHandler(this);
        }
    }
}