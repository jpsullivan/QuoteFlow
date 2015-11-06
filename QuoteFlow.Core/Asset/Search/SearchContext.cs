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
        #region DI

        protected ICatalogService CatalogService { get; set; }
        protected IManufacturerService ManufacturerService { get; set; }

        public SearchContext()
        {
        }

        public SearchContext(ICatalogService catalogService, IManufacturerService manufacturerService)
        {
            CatalogService = catalogService;
            ManufacturerService = manufacturerService;
        }

        public SearchContext(List<int?> catalogIds, List<int?> manufacturerIds)
        {
            CatalogIds = catalogIds;
            ManufacturerIds = manufacturerIds;
        }

        #endregion

        private static readonly List<int?> AllCatalogs = new List<int?>();
        private static readonly List<int?> AllManufacturers = new List<int?>(); 

        /// <summary>
        /// Returns whether the context is <em>global</em> or not. A context is global when there 
        /// are no catalog restrictions.
        /// </summary>
        public bool IsForAnyCatalogs()
        {
            return !CatalogIds.AnySafe();
        }

        /// <summary>
        /// Returns true if no specific manufacturers have been selected.
        /// </summary>
        /// <returns></returns>
        public bool IsForAnyManufacturers()
        {
            return !ManufacturerIds.AnySafe();
        }

        /// <summary>
        /// Returns true if there is exactly one Catalog in this SearchContext.
        /// </summary>
        public bool IsSingleCatalogContext()
        {
            return CatalogIds != null && CatalogIds.Count == 1;
        }

        /// <summary>
        /// Returns the single <see cref="Catalog"/> for this SearchContext.
        /// </summary>
        public Catalog SingleCatalog 
        {
            get
            {
                if (IsSingleCatalogContext())
                {
                    return CatalogService.GetCatalog((int) CatalogIds.First());
                }

                throw new InvalidOperationException("This is not a single catalog context.");
            } 
        }

        private List<int?> _catalogIds; 
        public List<int?> CatalogIds
        {
            get { return _catalogIds; }
            set 
            { 
                _catalogIds = value;
                Catalogs = null;
            }
        }

        private List<int?> _manufacturerIds;
        public List<int?> ManufacturerIds
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
                var contexts = new List<IAssetContext>();
                var catalogIds = (CatalogIds.AnySafe() ? CatalogIds : AllCatalogs);
                foreach (var catalogId in catalogIds)
                {
                    var manufacturerIds = (ManufacturerIds.AnySafe() ? ManufacturerIds : AllManufacturers);
                    contexts.AddRange(manufacturerIds.Select(id => new AssetContext(catalogId, id)));
                }

                return catalogIds.Select(catalogId => new AssetContext(catalogId));
            }
        } 

        public void Verify()
        {
            if (!CatalogIds.AnySafe()) return;
            
            foreach (var catalogId in CatalogIds)
            {
                if (CatalogService.GetCatalog((int) catalogId) == null)
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
                return _catalogs ?? (_catalogs = CatalogIds.Select(id => CatalogService.GetCatalog((int) id)));
            }
            set { _catalogs = value; }
        }

        private IEnumerable<Manufacturer> _manufacturers;
        public IEnumerable<Manufacturer> Manufacturers
        {
            get
            {
                if (ManufacturerIds == null) return null;
                return _manufacturers ?? (_manufacturers = ManufacturerIds.Select(id => ManufacturerService.GetManufacturer((int) id)));
            }
            set { _manufacturers = value; }
        }

        private bool Equals(ISearchContext other)
        {
            if ((_catalogIds == null && other.CatalogIds == null) && (_manufacturerIds == null && other.ManufacturerIds == null))
            {
                return true;
            }

            return _catalogIds.SequenceEqual(other.CatalogIds) && _manufacturerIds.SequenceEqual(other.ManufacturerIds);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SearchContext) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_catalogIds != null ? _catalogIds.GetHashCode() : 0)*397) ^
                       (_manufacturerIds != null ? _manufacturerIds.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("Catalog Ids: {0}", CatalogIds.ToString());
        }
    }
}