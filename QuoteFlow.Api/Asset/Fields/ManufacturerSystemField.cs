using System;
using System.Collections;
using System.Collections.Generic;
using QuoteFlow.Api.Asset.Search.Handlers;

namespace QuoteFlow.Api.Asset.Fields
{
    public class ManufacturerSystemField : AbstractOrderableNavigableField, IManufacturerField
    {
        private const string ManufacturerNameKey = "asset.field.manufacturer";

        public ManufacturerSystemField(ManufacturerSearchHandlerFactory searchHandlerFactory)
            : base(AssetFieldConstants.Manufacturer, ManufacturerNameKey, searchHandlerFactory)
		{
		}

        protected override object GetRelevantParams(IDictionary<string, string[]> parameters)
        {
            throw new NotImplementedException();
        }

        public override object GetValueFromParams(IDictionary fieldParams)
        {
            throw new NotImplementedException();
        }
    }
}