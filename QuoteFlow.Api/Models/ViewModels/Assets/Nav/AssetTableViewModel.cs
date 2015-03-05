using System.Collections.Generic;
using QuoteFlow.Api.Asset.Nav;

namespace QuoteFlow.Api.Models.ViewModels.Assets.Nav
{
    /// <summary>
    /// The outcome of a successful request to the asset table service, including
    /// both the resultin <see cref="AssetTable"/> and any warnings that were generated.
    /// </summary>
    public class AssetTableViewModel
    {
        public AssetTable AssetTable { get; set; }

        /// <summary>
        /// Gets all warnings that were generated while processing the request,
        /// e.g. "The value 'foo' does not exist for the field 'reporter'.
        /// </summary>
        public List<string> Warnings { get; set; }

        public AssetTableViewModel()
        {
        }

        public AssetTableViewModel(AssetTable assetTable, List<string> warnings)
        {
            AssetTable = assetTable;
            Warnings = warnings;
        }
    }
}
