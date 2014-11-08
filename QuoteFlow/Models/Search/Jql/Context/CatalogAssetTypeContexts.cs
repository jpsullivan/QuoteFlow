using System.Collections.Generic;

namespace QuoteFlow.Models.Search.Jql.Context
{
    public class CatalogAssetTypeContexts
    {
        private readonly CatalogContext catalog;

        public CatalogAssetTypeContexts(CatalogContext catalog)
		{
			this.catalog = catalog;
		}

		public virtual CatalogContext CatalogContext
		{
			get { return catalog; }
		}

		public virtual List<int?> CatalogIdInList
		{
			get
			{
				if (catalog.IsAll())
				{
					return new List<int?>();
				}
			    return new List<int?>() {catalog.CatalogId};
			}
		}
    }
}