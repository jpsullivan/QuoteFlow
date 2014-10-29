using System;
using System.Collections;
using System.Collections.Generic;
using QuoteFlow.Models.Assets.Search;

namespace QuoteFlow.Models.Assets.Fields
{
    public class ICustomField : IOrderableField
    {
        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public string Id { get; private set; }
        public string NameKey { get; private set; }
        public string Name { get; private set; }
        public SearchHandler CreateAssociatedSearchHandler()
        {
            throw new NotImplementedException();
        }

        public void PopulateDefaults(IDictionary<string, object> fieldValuesHolder, Asset asset)
        {
            throw new NotImplementedException();
        }

        public bool HasParam(IDictionary<string, string[]> parameters)
        {
            throw new NotImplementedException();
        }

        public void PopulateFromParams(IDictionary<string, object> fieldValuesHolder, IDictionary<string, string[]> parameters)
        {
            throw new NotImplementedException();
        }

        public void PopulateFromAsset(IDictionary<string, object> fieldValuesHolder, Asset asset)
        {
            throw new NotImplementedException();
        }

        public bool HasValue(Asset asset)
        {
            throw new NotImplementedException();
        }

        public object GetValueFromParams(IDictionary fieldParams)
        {
            throw new NotImplementedException();
        }

        public void PopulateParamsFromString(IDictionary<string, object> fieldValuesHolder, string stringValue, Asset asset)
        {
            throw new NotImplementedException();
        }
    }
}