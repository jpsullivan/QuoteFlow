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

        protected bool Equals(Manufacturer other)
        {
            return Id == other.Id && string.Equals(Name, other.Name) && string.Equals(Description, other.Description);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Manufacturer) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Description != null ? Description.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}