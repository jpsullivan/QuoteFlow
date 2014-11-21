using System;
using System.Collections;
using System.Collections.Generic;
using QuoteFlow.Models.Assets.Search;
using QuoteFlow.Models.Assets.Search.Handlers;

namespace QuoteFlow.Models.Assets.Fields
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

        public void PopulateDefaults(IDictionary<string, object> fieldValuesHolder, Asset asset)
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


        public void PopulateFromAsset(IDictionary<string, object> fieldValuesHolder, Asset asset)
        {
            throw new NotImplementedException();
        }

        public bool HasValue(Asset asset)
        {
            throw new NotImplementedException();
        }

        public abstract object GetValueFromParams(IDictionary fieldParams);

        public void PopulateParamsFromString(IDictionary<string, object> fieldValuesHolder, string stringValue, Asset asset)
        {
            throw new NotImplementedException();
        }
    }
}