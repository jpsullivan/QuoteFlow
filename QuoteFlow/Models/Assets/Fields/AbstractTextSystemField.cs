using System.Collections;
using System.Collections.Generic;
using QuoteFlow.Models.Assets.Search.Handlers;

namespace QuoteFlow.Models.Assets.Fields
{
    public abstract class AbstractTextSystemField : AbstractOrderableNavigableField
    {
        public AbstractTextSystemField(string id, string name, ISearchHandlerFactory searcherHandlerFactory) : base(id, name, searcherHandlerFactory)
        {
        }

        protected override object GetRelevantParams(IDictionary<string, string[]> parameters)
        {
            var value = parameters[base.Id];
            if (value != null && value.Length > 0)
            {
                return value[0];
            }

            return null;
        }

        public override object GetValueFromParams(IDictionary fieldParams)
        {
            return fieldParams[base.Id];
        }
    }
}