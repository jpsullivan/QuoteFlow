﻿using System;
using Lucene.Net.Documents;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Asset.Search.Constants;

namespace QuoteFlow.Core.Asset.Index.Indexers
{
    public class CatalogIdIndexer : BaseFieldIndexer
    {
        public override string Id
		{
			get { return SystemSearchConstants.ForCatalog().FieldId; }
		}

        public override string DocumentFieldId
		{
			get { return SystemSearchConstants.ForCatalog().IndexField; }
		}

		public override bool IsFieldVisibleAndInScope(Api.Models.Asset asset)
		{
			return true;
		}

		public override void AddIndex(Document doc, Api.Models.Asset asset)
		{
            IndexKeyword(doc, DocumentFieldId, Convert.ToString(asset.CatalogId), asset);
            // For sorting
		    string catalogName = asset.Catalog.Name;
            if (catalogName != null)
            {
                IndexKeyword(doc, DocumentConstants.CatalogId, catalogName, asset);
            }
		}
    }
}