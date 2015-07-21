using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Jql.Resolver;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Core.Jql.Resolver
{
    /// <summary>
    /// Resolves Manufacturer objects and IDs from their names.
    /// </summary>
    public class ManufacturerResolver : INameResolver<Manufacturer>
    {
        public IManufacturerService ManufacturerService { get; protected set; }

        public ManufacturerResolver(IManufacturerService manufacturerService)
        {
            if (manufacturerService == null)
            {
                throw new ArgumentNullException(nameof(manufacturerService));
            }

            ManufacturerService = manufacturerService;
        }

        public List<string> GetIdsFromName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var manufacturer = ManufacturerService.GetManufacturer(name);
            return manufacturer == null ? new List<string>() : new List<string> { manufacturer.Id.ToString() };
        }

        public bool NameExists(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var manufacturer = ManufacturerService.GetManufacturer(name);
            return manufacturer != null;
        }

        public bool IdExists(int id)
        {
            if (id == 0)
            {
                throw new ArgumentException("ID must be greater than zero.", nameof(id));
            }

            var manufacturer = ManufacturerService.GetManufacturer(id);
            return manufacturer != null;
        }

        public Manufacturer Get(int id)
        {
            return ManufacturerService.GetManufacturer(id);
        }

        public ICollection<Manufacturer> GetAll()
        {
            return ManufacturerService.GetManufacturers(1).ToList(); // todo: replace organization id
        }
    }
}