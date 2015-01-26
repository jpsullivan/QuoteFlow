using System;
using Wintellect.PowerCollections;

namespace QuoteFlow.Api.Asset.Transport
{
    public interface IActionParams : IFieldParams
    {
        MultiDictionary<string, String[]> KeysAndValues { get; }

        string[] AllValues { get; }
        string[] ValuesForNullKey { get; }
        string[] GetValuesForKey(string key);

        string FirstValueForNullKey { get; }
        string GetFirstValueForKey(string key);

        void Put(string id, string[] values);
    }
}