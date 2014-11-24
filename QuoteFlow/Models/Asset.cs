using System;
using System.Collections.Generic;
using Dapper;

namespace QuoteFlow.Models
{
    public class Asset : IAsset
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SKU { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime CreationDate { get; set; }

        private decimal _cost;
        public decimal Cost
        {
            get { return decimal.Round(_cost, 2, MidpointRounding.AwayFromZero); }
            set { _cost = value; }
        }
        public decimal Markup { get; set; }

        private decimal _price;
        [IgnoreProperty(false)]
        public decimal Price
        {
            get
            {
                var result = (Markup * Cost) + Cost;
                return decimal.Round(result, 2, MidpointRounding.AwayFromZero);
            }
        }

        public int CreatorId { get; set; }
        public User Creator { get; set; }

        public int ManufacturerId { get; set; }
        public Manufacturer Manufacturer { get; set; }

        public int CatalogId { get; set; }
        public Catalog Catalog { get; set; }

        /// <summary>
        /// The asset vars and their respective values associated with
        /// this particular asset.
        /// </summary>
        public IEnumerable<AssetVar> AssetVars { get; set; } 

        public IEnumerable<AssetComment> Comments { get; set; }
    }
}