using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using QuoteFlow.Models;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Services
{
    public class ManufacturerService : IManufacturerService
    {
        #region IoC

        public IManufacturerLogoService ManufacturerLogoService { get; protected set; }

        public ManufacturerService() { }

        public ManufacturerService(IManufacturerLogoService manufacturerLogoService)
        {
            ManufacturerLogoService = manufacturerLogoService;
        }

        #endregion

        /// <summary>
        /// Fetches a <see cref="Manufacturer"/> by its identifier.
        /// </summary>
        /// <param name="manufacturerId">The <see cref="Manufacturer"/> identifier.</param>
        /// <returns></returns>
        public Manufacturer GetManufacturer(int manufacturerId)
        {
            if (manufacturerId == 0)
            {
                throw new ArgumentException("Manufacturer ID must be greater than zero.", "manufacturerId");
            }

            var manufacturer = Current.DB.Manufacturers.Get(manufacturerId);
            manufacturer.Logo = ManufacturerLogoService.GetByManufacturerId(manufacturerId);
            
            return manufacturer;
        }

        /// <summary>
        /// Fetches a <see cref="Manufacturer"/> by its name.
        /// </summary>
        /// <param name="manufacturerName">The <see cref="Manufacturer"/> name.</param>
        /// <returns></returns>
        public Manufacturer GetManufacturer(string manufacturerName)
        {
            const string sql = "select * from Manufacturers where Name = @manufacturerName";
            var manufacturer = Current.DB.Query<Manufacturer>(sql, new { manufacturerName }).FirstOrDefault();

            if (manufacturer != null)
            {
                manufacturer.Logo = ManufacturerLogoService.GetByManufacturerId(manufacturer.Id);
            }
            
            return manufacturer;
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
        /// Retrieves all of the manufacturers that exist within a 
        /// specific organization.
        /// </summary>
        /// <param name="organizationId">The organization id</param>
        /// <returns></returns>
        public IEnumerable<Manufacturer> GetManufacturers(int organizationId)
        {
            const string sql = "select * from Manufacturers where OrganizationId = @organizationId";

            var manufacturers = Current.DB.Query<Manufacturer>(sql, new {organizationId}).ToList();
            foreach (var m in manufacturers)
            {
                m.Logo = ManufacturerLogoService.GetByManufacturerId(m.Id);
            }

            return manufacturers;
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

        /// <summary>
        /// Updates a manufacturer's details based on a <see cref="Snapshotter"/> diff.
        /// </summary>
        /// <param name="id">The Id of the manufacturer to update.</param>
        /// <param name="diff">The <see cref="Snapshotter"/> diff.</param>
        public void UpdateManufacturer(int id, DynamicParameters diff)
        {
            if (id == 0)
            {
                throw new ArgumentException("Manufacturer ID must be greater than zero.", "id");
            }

            Current.DB.Manufacturers.Update(id, diff);
        }
    }
}