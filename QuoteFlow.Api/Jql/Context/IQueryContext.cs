using System.Collections.Generic;
using System.Linq;

namespace QuoteFlow.Api.Jql.Context
{
    /// <summary>
    /// Represents a context of catalogs and manufacturers generated from a search query.
    /// </summary>
    public interface IQueryContext
    {
        IEnumerable<QueryContextCatalogManufacturerContexts> CatalogManufacturerContexts { get; set; }
    }

    public class QueryContextCatalogManufacturerContexts
    {
        public ICatalogContext CatalogContext { get; protected set; }
        public List<IManufacturerContext> ManufacturerContexts { get; protected set; }

        public QueryContextCatalogManufacturerContexts(ICatalogContext catalogContext, List<IManufacturerContext> manufacturerContexts)
        {
            CatalogContext = catalogContext;
            ManufacturerContexts = manufacturerContexts;
        }

        public IEnumerable<int?> GetCatalogIdInList()
        {
            return CatalogContext.IsAll() ? new List<int?>() : new List<int?> { CatalogContext.CatalogId };
        }

        public IEnumerable<int?> GetManufacturerIds()
        {
            var manufacturerIds = new List<int?>();
            foreach (var manufacturerContext in ManufacturerContexts)
            {
                if (!manufacturerContext.IsAll())
                {
                    manufacturerIds.Add(manufacturerContext.ManufacturerId);
                }
            }

            return manufacturerIds;
        }

        protected bool Equals(QueryContextCatalogManufacturerContexts other)
        {
            if (!ManufacturerContexts.SequenceEqual(other.ManufacturerContexts))
            {
                return false;
            }

            if (!Equals(CatalogContext, other.CatalogContext))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((QueryContextCatalogManufacturerContexts) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((CatalogContext != null ? CatalogContext.GetHashCode() : 0)*397) ^ (ManufacturerContexts != null ? ManufacturerContexts.GetHashCode() : 0);
            }
        }
    }
}