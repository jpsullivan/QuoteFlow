using System;

namespace QuoteFlow.Models
{
    public class ManufacturerLogo
    {
        public int Id { get; set; }
        public int ManufacturerId { get; set; }
        public string Url { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}