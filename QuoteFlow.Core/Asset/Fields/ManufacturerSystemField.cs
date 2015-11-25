using System;
using System.Collections;
using System.Collections.Generic;
using QuoteFlow.Api.Asset;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Statistics;
using QuoteFlow.Core.Asset.Search.Handlers;

namespace QuoteFlow.Core.Asset.Fields
{
    public class ManufacturerSystemField : AbstractOrderableNavigableField, IManufacturerField
    {
        private const string ManufacturerNameKey = "asset.field.manufacturer";

        public ManufacturerStatisticsMapper ManufacturerStatisticsMapper { get; protected set; }

        public ManufacturerSystemField(ManufacturerStatisticsMapper manufacturerStatisticsMapper, ManufacturerSearchHandlerFactory searchHandlerFactory)
            : base(AssetFieldConstants.Manufacturer, ManufacturerNameKey, searchHandlerFactory)
        {
            ManufacturerStatisticsMapper = manufacturerStatisticsMapper;
        }

        protected override object GetRelevantParams(IDictionary<string, string[]> parameters)
        {
            throw new NotImplementedException();
        }

        public override object GetValueFromParams(IDictionary fieldParams)
        {
            throw new NotImplementedException();
        }

        public override string DefaultSortOrder => NavigableFieldOrder.Descending;
    }
}