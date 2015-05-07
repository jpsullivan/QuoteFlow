using System.Collections.Generic;
using QuoteFlow.Api.Asset.Nav;

namespace QuoteFlow.Api.Models.ViewModels.Quotes
{
    public class QuoteBuilderViewModel
    {
        public QuoteBuilderViewModel(Quote quote, AssetTable assetTable, IEnumerable<string> visibleFieldNames, IEnumerable<string> visibleFunctionNames, IEnumerable<string> jqlReservedWords)
        {
            Quote = quote;
            AssetTable = assetTable;
            VisibleFieldNames = visibleFieldNames;
            VisibleFunctionNames = visibleFunctionNames;
            JqlReservedWords = jqlReservedWords;
        }

        public Quote Quote { get; set; }
        public AssetTable AssetTable { get; set; }

        public IEnumerable<string> VisibleFieldNames { get; set; }
        public IEnumerable<string> VisibleFunctionNames { get; set; } 
        public IEnumerable<string> JqlReservedWords { get; set; } 

    }
}
