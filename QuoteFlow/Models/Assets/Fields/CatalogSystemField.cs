using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuoteFlow.Models.Assets.Search.Handlers;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Models.Assets.Fields
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
            : base(AssetFieldConstants.Catalog, PROJECT_NAME_KEY, searcherHandlerFactory)
        {
            CatalogService = catalogService;
        }

        protected override object GetRelevantParams(IDictionary<string, string[]> parameters)
        {
            throw new NotImplementedException();
        }
    }
}