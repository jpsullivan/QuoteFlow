using System;

namespace QuoteFlow.Models
{
    public class AssetPrice
    {
        // for dapper materialization
        public AssetPrice() { }

        public AssetPrice(int id, int assetId, int catalogId, decimal cost, decimal markup, decimal price,
            DateTime creationDate, DateTime lastUpdated)
        {
            Id = id;
            AssetId = assetId;
            CatalogId = catalogId;
            Cost = cost;
            Markup = markup;
            Price = price;
            CreationDate = creationDate;
            LastUpdated = lastUpdated;
        }

        public int Id { get; set; }
        public int AssetId { get; set; }

        public int CatalogId { get; set; }

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