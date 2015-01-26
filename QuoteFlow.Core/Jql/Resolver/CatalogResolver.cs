using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Jql.Resolver;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Core.Jql.Resolver
{
    public class CatalogResolver : INameResolver<Catalog>
    {
        #region IoC

        public ICatalogService CatalogService { get; protected set; }

        public CatalogResolver() { }

        public CatalogResolver(ICatalogService catalogService)
        {
            CatalogService = catalogService;
        }

        #endregion

        public List<string> GetIdsFromName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            var catalogList = new List<string>();

            var catalog = CatalogService.GetCatalog(name);
            if (catalog == null)
            {
                return catalogList;
            }

            catalogList.Add(catalog.Id.ToString());

            return catalogList;
        }

        public bool NameExists(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            var catalog = CatalogService.GetCatalog(name);
            return catalog != null;
        }

        public bool IdExists(int id)
        {
            if (id == 0)
            {
                throw new ArgumentException("Catalog ID must be greater than zero", "name");
            }

            var catalog = CatalogService.GetCatalog(id);
            return catalog != null;
        }

        public Catalog Get(int id)
        {
            return CatalogService.GetCatalog(id);
        }

        public ICollection<Catalog> GetAll()
        {
            return CatalogService.GetCatalogs(1).ToList();
        }
    }
}