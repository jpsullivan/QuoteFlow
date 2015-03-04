using System.Collections.Generic;
using System.ComponentModel;
using QuoteFlow.Api.Asset.Fields.Layout.Column;
using QuoteFlow.Api.Configuration;

namespace QuoteFlow.Core.Configuration
{
    public class AssetTableServiceConfiguration : IAssetTableServiceConfiguration
    {
        private const int DEFAULT_NUM_TO_SHOW = 50;

        public bool AddDefaults { get; set; }

        public List<string> ColumnNames { get; set; }

        public string Context { get; set; }

        [DefaultValue(true)]
        public bool DisplayHeader { get; set; }

        [DefaultValue(true)]
        public bool EnableSorting { get; set; }

        [DefaultValue(true)]
        public bool IsPaging { get; set; }

        public string LayoutKey { get; set; }

        [DefaultValue(DEFAULT_NUM_TO_SHOW)]
        public int NumberToShow { get; set; }

        public string SelectedAssetKey { get; set; }

        [DefaultValue(true)]
        public bool ShowActions { get; set; }

        public string SortBy { get; set; }

        public int Start { get; set; }

        public string Title { get; set; }

        public ColumnConfig ColumnConfig { get; set; }
    }
}
