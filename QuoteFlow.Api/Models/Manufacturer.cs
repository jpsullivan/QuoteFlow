using System;
using QuoteFlow.Api.Asset;

namespace QuoteFlow.Api.Models
{
    public class Manufacturer : IAssetConstant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public string IconUrlHtml { get; set; }
        public int OrganizationId { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime CreationDate { get; set; }

        public ManufacturerLogo Logo { get; set; }
    }
}