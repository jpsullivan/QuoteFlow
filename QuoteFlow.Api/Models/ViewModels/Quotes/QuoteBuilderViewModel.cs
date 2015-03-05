using System.Collections.Generic;
using QuoteFlow.Api.Asset.Nav;

namespace QuoteFlow.Api.Models.ViewModels.Quotes
{
    public class QuoteBuilderViewModel
    {
        public QuoteBuilderViewModel(Quote quote, IEnumerable<Catalog> catalogs, IEnumerable<Manufacturer> manufacturers, 
            IEnumerable<User> creators, AssetTable assetTable)
        {
            Quote = quote;
            Catalogs = catalogs;
            Manufacturers = manufacturers;
            Creators = creators;
            AssetTable = assetTable;
        }

        public Quote Quote { get; set; }
        public IEnumerable<Catalog> Catalogs { get; set; }
        public IEnumerable<Manufacturer> Manufacturers { get; set; }
        public IEnumerable<User> Creators { get; set; }
        public AssetTable AssetTable { get; set; }
    }
}
