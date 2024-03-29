﻿using System;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Asset.Context
{
    public class AssetContext : IAssetContext, IComparable
    {
        /// <summary>
        /// Gets the <see cref="Catalog"/> for this context. A null return value is used to
        /// represent that this context applies to all catalogs.
        /// </summary>
        public Catalog CatalogObject { get; private set; }

        /// <summary>
        /// Gets the ID of the <see cref="Catalog"/> for this context. A null return value is used to
        /// represent that this context applies to all catalogs.
        /// </summary>
        public int? CatalogId { get; private set; }

        public int? ManufacturerId { get; private set; }

        public AssetContext(int? catalogId, int? manufacturerId)
        {
            CatalogId = catalogId;
            ManufacturerId = manufacturerId;
        }

        public AssetContext(Catalog catalogObject, int? catalogId)
        {
            CatalogObject = catalogObject;
            CatalogId = catalogId;
        }

        public AssetContext(Catalog catalog)
        {
            CatalogId = catalog?.Id;
        }

        public AssetContext(int? catalogId)
        {
            CatalogId = catalogId;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            var o = (AssetContext) obj;
            var comparable = CatalogId as IComparable;
            return comparable?.CompareTo(o.CatalogId) ?? 1;
        }

        public override string ToString()
        {
            return $"AssetContext[catalogId={CatalogId}]";
        }
    }
}