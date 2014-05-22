using System;
using System.Collections.Generic;

namespace QuoteFlow.Models
{
    public class Asset
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SKU { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int CreatorId { get; set; }
        public int OrganizationId { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime CreationDate { get; set; }

        public IEnumerable<AssetPrice> Prices { get; set; }

        public int ManufacturerId { get; set; }
        public Manufacturer Manufacturer { get; set; }
    }
}