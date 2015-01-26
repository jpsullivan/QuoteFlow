using System;
using System.Collections.Generic;

namespace QuoteFlow.Api.Models
{
    public interface IAsset
    {
        int Id { get; set; }
        string Name { get; set; }
        string SKU { get; set; }
        string Type { get; set; }
        string Description { get; set; }
        DateTime LastUpdated { get; set; }
        DateTime CreationDate { get; set; }

        decimal Cost { get; set; }
        decimal Price { get; }

        int CreatorId { get; set; }
        User Creator { get; set; }

        int ManufacturerId { get; set; }
        Manufacturer Manufacturer { get; set; }

        int CatalogId { get; set; }
        Catalog Catalog { get; set; }

        /// <summary>
        /// The asset vars and their respective values associated with
        /// this particular asset.
        /// </summary>
        IEnumerable<AssetVar> AssetVars { get; set; }

        IEnumerable<AssetComment> Comments { get; set; }
    }
}