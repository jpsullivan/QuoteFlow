using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Asset.Context;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Core.Asset.Search
{
    public class SearchContext : ISearchContext
    {
        #region IoC

        protected ICatalogService CatalogService { get; set; }
        protected IManufacturerService ManufacturerService { get; set; }

        public SearchContext() { }

        public SearchContext(ICatalogService catalogService, IManufacturerService manufacturerService)
        {
            CatalogService = catalogService;
            ManufacturerService = manufacturerService;
        }

        #endregion

        private static readonly List<int> AllCatalogs = new List<int>();
        private static readonly List<int> AllManufacturers = new List<int>(); 

        /// <summary>
        /// Returns whether the context is <em>global</em> or not. A context is global when there 
        /// are no catalog restrictions.
        /// </summary>
        public bool IsForAnyCatalogs()
        {
            return CatalogIds.AnySafe();
        }

        /// <summary>
        /// Returns true if no specific manufacturers have been selected.
        /// </summary>
        /// <returns></returns>
        public bool IsForAnyManufacturers()
        {
            return (manufacturerIds)
        }

        /// <summary>
        /// Returns true if there is exactly one Catalog in this SearchContext.
        /// </summary>
        public bool IsSingleCatalogContext() 
        {
            return CatalogIds.AnySafe();
        }

        /// <summary>
        /// Returns the single <see cref="Catalog"/> for this SearchContext.
        /// </summary>
        public Catalog SingleCatalog {
            get
            {
                if (IsSingleCatalogContext())
                {
                    return CatalogService.GetCatalog(CatalogIds.First());
                }

                throw new InvalidOperationException("This is not a single catalog context.");
            } 
        }

        private List<int> _catalogIds; 
        public List<int> CatalogIds
        {
            get { return _catalogIds; }
            set 
            { 
                _catalogIds = value;
                Catalogs = null;
            }
        }

        private List<int> _manufacturerIds;
        public List<int> ManufacturerIds
        {
            get { return _manufacturerIds; }
            set
            {
                _manufacturerIds = value;
                Manufacturers = null;
            }
        } 

        public IEnumerable<IAssetContext> AsAssetContexts
        {
            get
            {
                var catalogIds = (CatalogIds.AnySafe() ? CatalogIds : AllCatalogs);
                return catalogIds.Select(catalogId => new AssetContext(catalogId));
            }
        } 

        public void Verify()
        {
            if (!CatalogIds.AnySafe()) return;
            
            foreach (var catalogId in CatalogIds)
            {
                if (CatalogService.GetCatalog(catalogId) == null)
                {
                    // catalog no longer exists, remove it from the search context
                    CatalogIds.Remove(catalogId);
                }
            }
        }

        private IEnumerable<Catalog> _catalogs; 
        public IEnumerable<Catalog> Catalogs
        {
            get
            {
                if (CatalogIds == null) return null;
                return _catalogs ?? (_catalogs = CatalogIds.Select(id => CatalogService.GetCatalog(id)));
            }
            set { _catalogs = value; }
        }

        private IEnumerable<Manufacturer> _manufacturers;
        public IEnumerable<Manufacturer> Manufacturers
        {
            get
            {
                if (ManufacturerIds == null) return null;
                return _manufacturers ?? (_manufacturers = ManufacturerIds.Select(id => ManufacturerService.GetManufacturer(id)));
            }
            set { _manufacturers = value; }
        }

        public override string ToString()
        {
            return string.Format("Catalog Ids: {0}", CatalogIds.ToString());
        }
    }
}