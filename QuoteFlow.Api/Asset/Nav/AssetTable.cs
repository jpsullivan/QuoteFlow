using System.Collections.Generic;
using QuoteFlow.Api.Jql.Query.Order;

namespace QuoteFlow.Api.Asset.Nav
{
    public class AssetTable
    {
        public IDictionary<string, string> ColumnSortJql { get; set; }
        public string Description { get; set; }
        public int Displayed { get; set; }
        public int End { get; set; }
        public IList<int?> AssetIds { get; set; }
        public IList<string> AssetKeys { get; set; }
        public bool QuoteFlowHasAssets { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int StartIndex { get; set; }
        public object Table { get; set; }
        public string Title { get; set; }
        public int Total { get; set; }
        public string Url { get; set; }
        public SortBy SortBy { get; set; }
        public IEnumerable<string> Columns { get; set; }
        public string ColumnConfig { get; set; }
    }
}
