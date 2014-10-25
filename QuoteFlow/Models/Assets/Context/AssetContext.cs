using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteFlow.Models.Assets.Context
{
    public class AssetContext : IAssetContext, IComparable
    {
        /// <summary>
        /// Gets the <see cref="Asset"/> for this context.  A null return value is used to
        /// represent that this context applies to all catalogs.
        /// </summary>
        public Asset AssetObject { get; private set; }

        /// <summary>
        /// Gets the ID of the <see cref="Catalog"/> for this context. A null return value is used to
        /// represent that this context applies to all catalogs.
        /// </summary>
        public int? CatalogId { get; private set; }

        public AssetContext(Asset assetObject, int catalogId)
        {
            AssetObject = assetObject;
            CatalogId = catalogId;
        }

        public AssetContext(Catalog catalog)
        {
            CatalogId = catalog != null ? catalog.Id : (int?) null;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            var o = (AssetContext) obj;
            var comparable = CatalogId as IComparable;
            if (comparable != null) return comparable.CompareTo(o.CatalogId);
        }
    }
}