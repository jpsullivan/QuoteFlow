using System;
using System.Collections.Generic;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Asset
{
    public abstract class AbstractAsset : IAsset
    {
        public abstract int Id { get; set; }
        public abstract string Name { get; set; }
        public abstract string SKU { get; set; }
        public abstract string Type { get; set; }
        public abstract string Description { get; set; }
        public abstract DateTime LastUpdated { get; set; }
        public abstract DateTime CreationDate { get; set; }
        public abstract decimal Cost { get; set; }
        public abstract decimal Price { get; }
        public abstract int CreatorId { get; set; }
        public abstract User Creator { get; set; }
        public abstract int ManufacturerId { get; set; }
        public abstract Manufacturer Manufacturer { get; set; }
        public abstract int CatalogId { get; set; }
        public abstract Catalog Catalog { get; set; }
        public abstract IEnumerable<AssetVar> AssetVars { get; set; }
        public abstract IEnumerable<AssetComment> Comments { get; set; }
    }
}