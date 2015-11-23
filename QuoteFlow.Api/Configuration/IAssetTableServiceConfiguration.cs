using System.Collections.Generic;
using QuoteFlow.Api.Asset.Fields.Layout.Column;

namespace QuoteFlow.Api.Configuration
{
    public interface IAssetTableServiceConfiguration
    {
        /// <summary>
        /// Whether the default columns should be added.
        /// </summary>
        bool AddDefaults { get; set; }

        /// <summary>
        /// the columns that are to be displayed.
        /// </summary>
        List<string> ColumnNames { get; set; }

        /// <summary>
        /// The name of the application property that defines the default columns.
        /// </summary>
        string Context { get; set; }

        /// <summary>
        /// Whether the header should be displayed.
        /// </summary>
        bool DisplayHeader { get; set; }

        /// <summary>
        /// Whether sorting columns should be enabled.
        /// </summary>
        bool EnableSorting { get; set; }

        /// <summary>
        /// Whether pagination should be enabled.
        /// </summary>
        bool IsPaging { get; set; }

        /// <summary>
        /// The key of the layout to render.
        /// </summary>
        string LayoutKey { get; set; }

        /// <summary>
        /// The number of assets to show on a page.
        /// </summary>
        int NumberToShow { get; set; }

        /// <summary>
        /// The ID of the selected asset; if given, the page containing this
        /// asset is rendered and start is ignored.
        /// </summary>
        int? SelectedAssetId { get; set; }

        /// <summary>
        /// Whether the actions (cog) column should be shown.
        /// </summary>
        bool ShowActions { get; set; }

        string SortBy { get; set; }

        /// <summary>
        /// The index of the first asset that is to be displayed.
        /// </summary>
        int Start { get; set; }

        string Title { get; set; }

        /// <summary>
        /// The requested columns used to generate the asset table.
        /// </summary>
        ColumnConfig ColumnConfig { get; set; }
    }
}
