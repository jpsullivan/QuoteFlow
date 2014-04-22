using System;
using System.Linq;
using QuoteFlow.Models;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Services
{
    public class ManufacturerService : IManufacturerService
    {
        /// <summary>
        /// Fetches a <see cref="Manufacturer"/> by its identifier.
        /// </summary>
        /// <param name="manufacturerId">The <see cref="Manufacturer"/> identifier.</param>
        /// <returns></returns>
        public Manufacturer GetManufacturer(int manufacturerId)
        {
            return Current.DB.Manufacturers.Get(manufacturerId);
        }

        /// <summary>
        /// Fetches a <see cref="Manufacturer"/> by its name.
        /// </summary>
        /// <param name="manufacturerName">The <see cref="Manufacturer"/> name.</param>
        /// <returns></returns>
        public Manufacturer GetManufacturer(string manufacturerName)
        {
            const string sql = "select * from Manufacturers where Name = @manufacturerName";
            return Current.DB.Query<Manufacturer>(sql, new { manufacturerName }).FirstOrDefault();
        }

        /// <summary>
        /// Fetches a <see cref="Manufacturer"/> by its name and whether or not it should
        /// upsert through to the database if it does not exist.
        /// </summary>
        /// <param name="manufacturerName">The <see cref="Manufacturer"/> name.</param>
        /// <param name="upsert">True if the manufacturer should quickly be created if it doesn't exist.</param>
        /// <param name="organizationId">The organiation to set the created manufacturer to</param>
        /// <returns></returns>
        public Manufacturer GetManufacturer(string manufacturerName, bool upsert, int organizationId = 0)
        {
            var mfc = GetManufacturer(manufacturerName);

            if (mfc != null || !upsert) return mfc;

            if (organizationId > 0) {
                mfc = CreateManufacturer(manufacturerName, organizationId);
            }

            return mfc;
        }

        /// <summary>
        /// Inserts a new <see cref="Manufacturer"/> into the database.
        /// </summary>
        /// <param name="name">The <see cref="Manufacturer"/> name.</param>
        /// <param name="organizationId">The <see cref="Organization"/> identifier.</param>
        /// <param name="description">The <see cref="Manufacturer"/> description.</param>
        /// <returns></returns>
        public Manufacturer CreateManufacturer(string name, int organizationId, string description = null)
        {
            var manufacturer = new Manufacturer
            {
                Name = name,
                Description = description,
                CreationDate = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                OrganizationId = organizationId
            };

            var insert = Current.DB.Manufacturers.Insert(new
            {
                manufacturer.Name,
                manufacturer.Description,
                manufacturer.OrganizationId,
                manufacturer.CreationDate,
                manufacturer.LastUpdated
            });

            if (insert != null)
            {
                manufacturer.Id = insert.Value;
            }

            return manufacturer;
        }
    }
}