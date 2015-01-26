using System.Collections.Generic;
using QuoteFlow.Api.Infrastructure.Helpers;

namespace QuoteFlow.Api.Models.ViewModels.Assets
{
    public class InteractiveAssetNavigatorViewModel
    {
        public AssetDetailsModel CurrentAssetDetailsModel { get; set; }

        public PagedList<Asset> Assets { get; set; }

        /// <summary>
        /// A collection of all the asset creators found within this particular
        /// group of assets.
        /// todo: Should this collection contain a list of the organization members instead?
        /// </summary>
        public IEnumerable<User> Creators { get; set; }

        /// <summary>
        /// The distinct list of manufacturers that are contained within 
        /// this catalog set.
        /// </summary>
        public IEnumerable<Manufacturer> Manufacturers { get; set; }

        public string PaginationUrl { get; set; }
    }
}