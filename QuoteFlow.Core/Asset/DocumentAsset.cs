using System;
using System.Collections;
using System.Collections.Generic;
using Lucene.Net.Documents;
using Ninject;
using QuoteFlow.Api.Asset;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;
using Container = QuoteFlow.Core.DependencyResolution.Container;

namespace QuoteFlow.Core.Asset
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
            FieldManager = Container.Kernel.TryGet<IFieldManager>();
            AssetService = assetService;
            CatalogService = catalogService;
        }

        public override int Id
        {
            get { return Convert.ToInt32(Document.Get(DocumentConstants.AssetId)); }
            set { throw new NotImplementedException(); }
        }

        public override string Name
        {
            get { return Document.Get(DocumentConstants.AssetName); }
            set { throw new NotImplementedException(); }
        }

        public override string SKU { get; set; }
        public override string Type { get; set; }

        public override string Description
        {
            get { return Document.Get(DocumentConstants.AssetDesc); }
            set { throw new NotImplementedException(); }
        }

        public override DateTime LastUpdated
        {
            get { return Convert.ToDateTime(Document.Get(DocumentConstants.AssetUpdated)); }
            set { throw new NotImplementedException(); }
        }

        public override DateTime CreationDate
        {
            get { return Convert.ToDateTime(Document.Get(DocumentConstants.AssetCreated)); }
            set { throw new NotImplementedException(); }
        }

        public override decimal Cost { get; set; }

        public override decimal Price
        {
            get { throw new NotImplementedException(); }
        }

        public override int CreatorId
        {
            get { return Convert.ToInt32(Document.Get(DocumentConstants.AssetCreator)); }
            set { throw new NotImplementedException(); }
        }
        public override User Creator
        {
            get { return (User) GetSingleValueFromField(AssetFieldConstants.Creator); }
            set { throw new NotImplementedException(); }
        }
        
        public override int ManufacturerId { get; set; }
        public override Manufacturer Manufacturer { get; set; }

        public override int CatalogId
        {
            get
            {
                var catalog = Catalog;
                return catalog != null ? catalog.Id : 0;
            }
            set { throw new NotImplementedException(); }
        }
        public override Catalog Catalog
        {
            get { return (Catalog) GetSingleValueFromField(AssetFieldConstants.Catalog); }
            set { throw new NotImplementedException(); }
        }

        public override IEnumerable<AssetVar> AssetVars { get; set; }
        public override IEnumerable<AssetComment> Comments { get; set; }

        private object GetSingleValueFromField(string fieldName)
        {
            ILuceneFieldSorter<object> sorter = FieldManager.GetNavigableField(fieldName).Sorter;
            return sorter.GetValueFromLuceneField(Document.Get(sorter.DocumentConstant));
        }

        private IList GetMultipleValuesFromField(string fieldName)
        {
            ILuceneFieldSorter<object> sorter = FieldManager.GetNavigableField(fieldName).Sorter;
            string[] documentValues = Document.GetValues(sorter.DocumentConstant);
            if (documentValues == null || documentValues.Length == 0)
            {
                return new ArrayList();
            }

            var values = new List<object>();
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