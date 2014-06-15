using System.Collections.Generic;
using Dapper;
using QuoteFlow.Models;

namespace QuoteFlow.Services.Interfaces
{
    public interface IManufacturerService
    {
        /// <summary>
        /// Fetches a <see cref="Manufacturer"/> by its identifier.
        /// </summary>
        /// <param name="manufacturerId">The <see cref="Manufacturer"/> identifier.</param>
        /// <returns></returns>
        Manufacturer GetManufacturer(int manufacturerId);

        /// <summary>
        /// Fetches a <see cref="Manufacturer"/> by its name.
        /// </summary>
        /// <param name="manufacturerName">The <see cref="Manufacturer"/> name.</param>
        /// <returns></returns>
        Manufacturer GetManufacturer(string manufacturerName);

        /// <summary>
        /// Fetches a <see cref="Manufacturer"/> by its name and whether or not it should
        /// upsert through to the database if it does not exist.
        /// </summary>
        /// <param name="manufacturerName">The <see cref="Manufacturer"/> name.</param>
        /// <param name="upsert">True if the manufacturer should quickly be created if it doesn't exist.</param>
        /// <param name="organizationId">The organiation to set the created manufacturer to</param>
        /// <returns></returns>
        Manufacturer GetManufacturer(string manufacturerName, bool upsert, int organizationId = 0);

        /// <summary>
        /// Retrieves all of the manufacturers that exist within a 
        /// specific organization.
        /// </summary>
        /// <param name="organizationId">The organization id</param>
        /// <returns></returns>
        IEnumerable<Manufacturer> GetManufacturers(int organizationId);

        /// <summary>
        /// Inserts a new <see cref="Manufacturer"/> into the database.
        /// </summary>
        /// <param name="name">The <see cref="Manufacturer"/> name.</param>
        /// <param name="organizationId">The <see cref="Organization"/> identifier.</param>
        /// <param name="description">The <see cref="Manufacturer"/> description.</param>
        /// <returns></returns>
        Manufacturer CreateManufacturer(string name, int organizationId, string description = null);

        /// <summary>
        /// Updates a manufacturer's details based on a <see cref="Snapshotter"/> diff.
        /// </summary>
        /// <param name="id">The Id of the manufacturer to update.</param>
        /// <param name="diff">The <see cref="Snapshotter"/> diff.</param>
        void UpdateManufacturer(int id, DynamicParameters diff);
    }
}