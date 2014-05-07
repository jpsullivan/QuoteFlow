using System;

namespace QuoteFlow.Models
{
    public class Catalog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int OrganizationId { get; set; }
        public int CreatorId { get; set; }

        /// <summary>
        /// The date when the prices bound to the catalog
        /// are allowed to be used.
        /// </summary>
        public DateTime? EffectiveDate { get; set; }

        public DateTime? ExpirationDate { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool Enabled { get; set; }

        public User Creator { get; set; }

        /// <summary>
        /// The total number of assets that are assigned to this catalog.
        /// </summary>
        public int TotalAssets { get; set; }
    }
}