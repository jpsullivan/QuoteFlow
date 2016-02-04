using System;
using QuoteFlow.Api.Jql.Context;

namespace QuoteFlow.Core.Jql.Context
{
    public class ManufacturerContext : IManufacturerContext
    {
        public int? ManufacturerId { get; private set; }

        public ManufacturerContext(int manufacturerId)
        {
            if (manufacturerId == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(manufacturerId), "manufacturerId must be greater than zero.");
            }

            ManufacturerId = manufacturerId;
        }

        public bool IsAll()
        {
            return false;
        }

        protected bool Equals(ManufacturerContext other)
        {
            return ManufacturerId == other.ManufacturerId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ManufacturerContext) obj);
        }

        public override int GetHashCode()
        {
            return ManufacturerId.GetHashCode();
        }

        public override string ToString()
        {
            return $"ManufacturerId: {ManufacturerId}";
        }
    }
}