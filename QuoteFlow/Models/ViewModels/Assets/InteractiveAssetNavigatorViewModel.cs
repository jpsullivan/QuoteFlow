using System.Collections.Generic;

namespace QuoteFlow.Models.ViewModels.Assets
{
    public class InteractiveAssetNavigatorViewModel
    {
        public AssetDetailsModel CurrentAssetDetailsModel { get; set; }

        public IEnumerable<Asset> Assets { get; set; } 
    }
}