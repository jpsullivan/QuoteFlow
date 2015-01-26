using System;
using System.Collections;
using System.Collections.Generic;
using QuoteFlow.Api.Asset;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Asset.CustomFields.Searchers.Transformer;

namespace QuoteFlow.Core.Asset.Fields
{
    /// <summary>
    /// QuoteFlow's catalog system field.
    /// </summary>
    public class CatalogSystemField : AbstractOrderableNavigableField, ICatalogField
    {
        public const string PROJECT_NAME_KEY = "asset.field.catalog";
        private const string FIELD_PARAMETER_NAME = "cid";

        public ICatalogService CatalogService { get; protected set; }

        public CatalogSystemField(ICatalogService catalogService, CatalogSearchHandlerFactory searchHandlerFactory)
            : base(AssetFieldConstants.Catalog, PROJECT_NAME_KEY, searchHandlerFactory)
        {
            CatalogService = catalogService;
        }

        protected override object GetRelevantParams(IDictionary<string, string[]> parameters)
        {
            string[] value = parameters[FIELD_PARAMETER_NAME];
            if (value != null && value.Length > 0)
            {
                return Convert.ToInt32(value[0]);
            }
            return null;
        }

        protected virtual Catalog GetCatalog(int catalogId)
        {
            return CatalogService.GetCatalog(catalogId);
        }

        public override object GetValueFromParams(IDictionary fieldParams)
        {
            return GetCatalog((int)fieldParams[Id]);
        }

        void IOrderableField.PopulateParamsFromString(IDictionary<string, object> fieldValuesHolder, string stringValue, Api.Models.Asset asset)
        {
            throw new NotImplementedException();
        }

        public virtual void PopulateForMove(IDictionary<string, object> fieldValuesHolder, Api.Models.Asset originalAsset, Api.Models.Asset targetAsset)
        {
            throw new NotImplementedException();
        }

        public virtual bool CanRemoveValueFromAssetObject(Api.Models.Asset asset)
        {
            return false;
        }

        string INavigableField.ColumnHeadingKey
        {
            get { return "asset.column.heading.catalog"; }
        }

        string INavigableField.DefaultSortOrder
        {
            get { return NavigableFieldOrder.Ascending; }
        }
    }
}