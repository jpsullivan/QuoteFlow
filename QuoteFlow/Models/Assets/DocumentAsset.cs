﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Lucene.Net.Documents;
using QuoteFlow.Models.Assets.Fields;
using QuoteFlow.Models.Assets.Index;
using QuoteFlow.Models.Assets.Search;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Models.Assets
{
    public class DocumentAsset : AbstractAsset
    {
        public Document Document { get; protected set; }
        public IFieldManager FieldManager { get; protected set; }
        public IAssetService AssetService { get; protected set; }
        public ICatalogService CatalogService { get; protected set; }

        private IDictionary _customFieldValues = new Hashtable();

        public DocumentAsset(Document document, IFieldManager fieldManager, IAssetService assetService, ICatalogService catalogService)
        {
            Document = document;
            FieldManager = fieldManager;
            AssetService = assetService;
            CatalogService = catalogService;
        }

        public override int Id
        {
            get { return Convert.ToInt32(Document.Get(DocumentConstants.AssetId)); }
            set { throw new NotImplementedException(); }
        }

        public override string Name { get; set; }
        public override string SKU { get; set; }
        public override string Type { get; set; }
        public override string Description { get; set; }
        public override DateTime LastUpdated { get; set; }
        public override DateTime CreationDate { get; set; }
        public override decimal Cost { get; set; }

        public override decimal Price
        {
            get { throw new NotImplementedException(); }
        }

        public override int CreatorId { get; set; }
        public override User Creator { get; set; }
        public override int ManufacturerId { get; set; }
        public override Manufacturer Manufacturer { get; set; }
        public override int CatalogId { get; set; }
        
        public override Catalog Catalog
        {
            get { }
            set { throw new NotImplementedException(); }
        }

        public override IEnumerable<AssetVar> AssetVars { get; set; }
        public override IEnumerable<AssetComment> Comments { get; set; }

        private object getSingleValueFromField(string fieldName)
        {
            ILuceneFieldSorter<object> sorter = FieldManager.GetNavigableField(fieldName).Sorter;
            return sorter.GetValueFromLuceneField(Document.Get(sorter.DocumentConstant));
        }

        private IList getMultipleValuesFromField(string fieldName)
        {
            ILuceneFieldSorter<object> sorter = FieldManager.GetNavigableField(fieldName).Sorter;
            string[] documentValues = Document.GetValues(sorter.DocumentConstant);
            if (documentValues == null || documentValues.Length == 0)
            {
                return new ArrayList();
            }

            IList values = new ArrayList();
            foreach (string documentValue in documentValues)
            {
                object value = sorter.GetValueFromLuceneField(documentValue);
                if (value != null)
                {
                    values.Add(value);
                }
            }
            values.Sort(sorter.Comparator);
            return values;
        }

    }
}