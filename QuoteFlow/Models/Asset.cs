﻿using System;

namespace QuoteFlow.Models
{
    public class Asset
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        private decimal _cost;
        public decimal Cost
        {
            get { return decimal.Round(_cost, 2, MidpointRounding.AwayFromZero); }
            set { _cost = value; }
        }

        public string Description { get; set; }
        public decimal Markup { get; set; }

        private decimal _price;
        public decimal Price
        {
            get { return decimal.Round(_price, 2, MidpointRounding.AwayFromZero); }
            set { _price = value; }
        }

        public int ManufacturerId { get; set; }
        public Manufacturer Manufacturer { get; set; }

        public int CreatorId { get; set; }

        public int CatalogId { get; set; }
        public Catalog Catalog { get; set; }

        public DateTime LastUpdated { get; set; }
        public DateTime CreationDate { get; set; }
    }
}