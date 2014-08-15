using System;
using System.Linq;
using QuoteFlow.Models;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Services
{
    public class ManufacturerLogoService : IManufacturerLogoService
    {
        /// <summary>
        /// Gets a single <see cref="ManufacturerLogo"/> based on the supplied 
        /// manufacturer identifier.
        /// </summary>
        /// <param name="manufacturerId"></param>
        /// <returns></returns>
        public ManufacturerLogo GetByManufacturerId(int manufacturerId)
        {
            if (manufacturerId == 0)
            {
                throw new ArgumentException("Manufacturer ID must be greater than zero", "manufacturerId");
            }

            const string sql = "select * from ManufacturerLogos where manufacturerId = @manufacturerId";
            return Current.DB.Query<ManufacturerLogo>(sql, new {manufacturerId}).FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manufacturerId"></param>
        /// <param name="token"></param>
        /// <param name="url"></param>
        public void CreateManufacturerLogo(int manufacturerId, Guid token, string url)
        {
            var logo = new ManufacturerLogo
            {
                ManufacturerId = manufacturerId,
                Token = token,
                Url = url,
                CreationDate = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };

            var insert = Current.DB.ManufacturerLogos.Insert(logo);
        }

        /// <summary>
        /// Deletes a <see cref="ManufacturerLogo"/> based on its supplied identifier.
        /// </summary>
        /// <param name="manufacturerId"></param>
        public void Delete(int manufacturerId)
        {
            if (manufacturerId == 0)
            {
                throw new ArgumentException("Manufacturer ID must be greater than zero", "manufacturerId");
            }

            Current.DB.ManufacturerLogos.Delete(manufacturerId);
        }
    }
}