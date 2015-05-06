using System.Collections.Generic;
using Jil;
using QuoteFlow.Api.Jql.Query.Order;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Asset.Nav
{
    public class AssetTable
    {
        [JilDirective("columnSortJql")]
        public IDictionary<string, string> ColumnSortJql { get; set; }

        [JilDirective("description")]
        public string Description { get; set; }

        [JilDirective("displayed")]
        public int Displayed { get; set; }

        [JilDirective("end")]
        public int End { get; set; }

        [JilDirective("assetIds")]
        public IList<int?> AssetIds { get; set; }

        [JilDirective("assetKeys")]
        public IList<string> AssetKeys { get; set; }

        [JilDirective("quoteFlowHasAssets")]
        public bool QuoteFlowHasAssets { get; set; }

        [JilDirective("page")]
        public int Page { get; set; }

        [JilDirective("pageSize")]
        public int PageSize { get; set; }

        [JilDirective("startIndex")]
        public int StartIndex { get; set; }

        [JilDirective("table")]
        public IEnumerable<AssetTableRow> Table { get; set; }

        [JilDirective("title")]
        public string Title { get; set; }

        [JilDirective("total")]
        public int Total { get; set; }

        [JilDirective("url")]
        public string Url { get; set; }

        [JilDirective("sortBy")]
        public SortBy SortBy { get; set; }

        [JilDirective("columns")]
        public IEnumerable<string> Columns { get; set; }

        [JilDirective("columnConfig")]
        public string ColumnConfig { get; set; }
    }
}
