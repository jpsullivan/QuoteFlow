using System.Collections.Generic;

namespace QuoteFlow.Models.Search.Jql.Context
{
    /// <summary>
    /// Represents a context of catalogs and manufacturers generated from a search query.
    /// </summary>
    public interface IQueryContext
    {
        IEnumerable<CatalogAssetTypeContexts> CatalogAssetTypeContexts { get; set; }
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

        public IEnumerable<int?> CatalogIdInList
        {
            get
            {
                if (CatalogContext.IsAll())
                {
                    return new List<int?>();
                }
                return new List<int?>() {CatalogContext.CatalogId};
            }
        }

        public IEnumerable<int> ManufacturerIds
        {
            get
            {
                var manufacturerIds = new List<int>();
                foreach (var manufacturerContext in ManufacturerContexts)
                {
                    if (!manufacturerContext.All)
                    {
                        manufacturerIds.Add(manufacturerContext.ManufacturerId);
                    }
                }
                return manufacturerIds;
            }
        } 
    }
}