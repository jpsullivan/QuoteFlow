using System;

namespace QuoteFlow.Models.Search.Jql.Context
{
    public class ManufacturerContext : IManufacturerContext
    {
        public int ManufacturerId { get; private set; }
        public bool All { get { return false; } }

        public ManufacturerContext(int manufacturerId)
        {
            if (manufacturerId == 0)
            {
                throw new ArgumentOutOfRangeException("manufacturerId", "manufacturerId must be greater than zero.");
            }

            ManufacturerId = manufacturerId;
        }
    }
}