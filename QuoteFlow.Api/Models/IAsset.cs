using System;

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
        int ManufacturerId { get; set; }
        int CatalogId { get; set; }
    }
}