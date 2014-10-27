using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Infrastructure.Extensions;
using QuoteFlow.Models.Assets.Context;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Models.Assets.Search
{
    public class SearchContext : ISearchContext
    {
        #region IoC

        protected ICatalogService CatalogService { get; set; }

        public SearchContext() { }

        public SearchContext(ICatalogService catalogService)
        {
            CatalogService = catalogService;
        }

        #endregion

        private static readonly List<int> AllCatalogs = new List<int>(); 

        /// <summary>
        /// Returns whether the context is <em>global</em> or not. A context is global when there 
        /// are no catalog restrictions.
        /// </summary>
        public bool IsForAnyCatalogs()
        {
            return CatalogIds.AnySafe();
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
            set { 
                _catalogIds = value;
                Catalogs = null;
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
            if (CatalogIds.AnySafe())
            {
                foreach (var catalogId in CatalogIds)
                {
                    if (CatalogService.GetCatalog(catalogId) == null)
                    {
                        // catalog no longer exists, remove it from the search context
                        CatalogIds.Remove(catalogId);
                    }
                }
            }
        }

        public IEnumerable<Catalog> Catalogs { get; set; }

        public override string ToString()
        {
            return string.Format("Catalog Ids: {0}", CatalogIds.ToString());
        }
    }
}