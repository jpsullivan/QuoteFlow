using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Jql.Resolver;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Jql.Resolver
{
    public class CatalogIndexInfoResolver : IIndexInfoResolver<Catalog>
    {
        #region IoC

        public INameResolver<Catalog> NameResolver { get; protected set; }

        public CatalogIndexInfoResolver() { }

        public CatalogIndexInfoResolver(INameResolver<Catalog> nameResolver)
        {
            NameResolver = nameResolver;
        }

        #endregion

        public List<string> GetIndexedValues(string rawValue)
        {
            if (rawValue == null)
            {
                throw new ArgumentNullException(nameof(rawValue));
            }

            var catalogs = NameResolver.GetIdsFromName(rawValue);
            if (catalogs.Any())
            {
                return catalogs;
            }
            
            var catalogId = Convert.ToInt32(rawValue);
            if (catalogId <= 0) return catalogs;
                
            var catalog = NameResolver.Get(catalogId);
            return catalog != null ? new List<string> {catalog.Id.ToString()} : catalogs;
        }

        public List<string> GetIndexedValues(int? rawValue)
        {
            if (rawValue == 0)
            {
                throw new ArgumentException("Raw value must be greater than zero.", nameof(rawValue));
            }

            var catalog = NameResolver.Get(rawValue.Value);
            if (catalog == null)
            {
                return NameResolver.GetIdsFromName(rawValue.ToString());
            }

            return new List<string> { catalog.Id.ToString() };
        }

        public string GetIndexedValue(Catalog catalog)
        {
            if (catalog == null)
            {
                throw new ArgumentNullException(nameof(catalog));
            }

            return catalog.Id.ToString();
        }
    }
}