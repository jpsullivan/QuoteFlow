using System;

namespace QuoteFlow.Models
{
    public class AssetPrice
    {
        public int Id { get; set; }
        public int AssetId { get; set; }

        public int CatalogId { get; set; }
        public Catalog Catalog { get; set; }

        private decimal _cost;
        public decimal Cost
        {
            get { return decimal.Round(_cost, 2, MidpointRounding.AwayFromZero); }
            set { _cost = value; }
        }
        public decimal Markup { get; set; }

        private decimal _price;
        public decimal Price
        {
            get { return decimal.Round(_price, 2, MidpointRounding.AwayFromZero); }
            set { _price = value; }
        }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}