using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteFlow.Models
{
    public class Catalog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int OrganizationId { get; set; }
        public int CreatorId { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool Enabled { get; set; }
    }
}