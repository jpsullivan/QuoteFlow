using System;
using System.Collections;
using System.Collections.Generic;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Handlers;

namespace QuoteFlow.Core.Asset.Fields
{
    public abstract class AbstractOrderableField : AbstractField, IOrderableField
    {
        public ISearchHandlerFactory SearcherHandlerFactory { get; protected set; }

        public AbstractOrderableField(string id, string name, ISearchHandlerFactory searcherHandlerFactory)
            : base(id, name)
        {
            SearcherHandlerFactory = searcherHandlerFactory;
        }

        public SearchHandler CreateAssociatedSearchHandler()
        {
            return SearcherHandlerFactory == null ? null : SearcherHandlerFactory.CreateHandler(this);
        }

        public void PopulateDefaults(IDictionary<string, object> fieldValuesHolder, Api.Models.Asset asset)
        {
            throw new NotImplementedException();
        }

        bool IOrderableField.HasParam(IDictionary<string, string[]> parameters)
        {
            return parameters.ContainsKey(Id);
        }

        void IOrderableField.PopulateFromParams(IDictionary<string, object> fieldValuesHolder, IDictionary<string, string[]> parameters)
        {
            fieldValuesHolder.Add(Id, GetRelevantParams(parameters));
        }

        protected abstract object GetRelevantParams(IDictionary<string, String[]> parameters);


        public void PopulateFromAsset(IDictionary<string, object> fieldValuesHolder, Api.Models.Asset asset)
        {
            throw new NotImplementedException();
        }

        public bool HasValue(Api.Models.Asset asset)
        {
            throw new NotImplementedException();
        }

        public abstract object GetValueFromParams(IDictionary fieldParams);

        public void PopulateParamsFromString(IDictionary<string, object> fieldValuesHolder, string stringValue, Api.Models.Asset asset)
        {
            throw new NotImplementedException();
        }
    }
}