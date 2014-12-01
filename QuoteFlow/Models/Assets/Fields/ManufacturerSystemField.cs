using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuoteFlow.Models.Assets.Search.Handlers;

namespace QuoteFlow.Models.Assets.Fields
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