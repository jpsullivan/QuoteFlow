using System.Collections.Generic;

namespace QuoteFlow.Api.Models.ViewModels.Quotes
{
    public class QuoteBuilderViewModel
    {
        public QuoteBuilderViewModel(Quote quote, IEnumerable<Catalog> catalogs, IEnumerable<Manufacturer> manufacturers, IEnumerable<User> creators)
        {
            Quote = quote;
            Catalogs = catalogs;
            Manufacturers = manufacturers;
            Creators = creators;
        }

        public Quote Quote { get; set; }
        public IEnumerable<Catalog> Catalogs { get; set; }
        public IEnumerable<Manufacturer> Manufacturers { get; set; }
        public IEnumerable<User> Creators { get; set; }
    }
}
