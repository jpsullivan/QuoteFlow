using System;

namespace QuoteFlow.Models
{
    public class Manufacturer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int OrganizationId { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime CreationDate { get; set; }

        public ManufacturerLogo Logo { get; set; }
    }
}