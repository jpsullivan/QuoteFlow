using System;
using System.Collections.Generic;
using QuoteFlow.Api.Jql.Resolver;

namespace QuoteFlow.Core.Jql.Resolver
{
    /// <summary>
    /// Cost ratios are stored as integers, but are padded in a specific way.
    /// </summary>
    public class CostIndexInfoResolver : IIndexInfoResolver<object>
    {
        public List<string> GetIndexedValues(string rawValue)
        {
            if (rawValue == null) throw new ArgumentNullException(nameof(rawValue));

            return new List<string> { rawValue };
        }

        public List<string> GetIndexedValues(int? rawValue)
        {
            if (rawValue == null) throw new ArgumentNullException(nameof(rawValue));
            return new List<string> {rawValue.Value.ToString()};
        }

        public string GetIndexedValue(object indexedObject)
        {
            // should never be called for cost ratios
            return null;
        }
    }
}