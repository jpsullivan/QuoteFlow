using System.Collections.Generic;

namespace QuoteFlow.Models.ViewModels.Assets
{
    public class InteractiveAssetNavigatorViewModel
    {
        public AssetDetailsModel CurrentAssetDetailsModel { get; set; }

        public IEnumerable<Asset> Assets { get; set; }

        /// <summary>
        /// The distinct list of manufacturers that are contained within 
        /// this catalog set.
        /// </summary>
        public IEnumerable<Manufacturer> Manufacturers { get; set; }

        public string PaginationUrl { get; set; }
    }
}