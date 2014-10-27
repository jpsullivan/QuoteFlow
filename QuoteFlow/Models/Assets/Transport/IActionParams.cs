using System;
using System.Collections.Generic;
using Wintellect.PowerCollections;

namespace QuoteFlow.Models.Assets.Transport
{
    public interface IActionParams : IFieldParams
    {
        MultiDictionary<string, String[]> KeysAndValues { get; }

        ICollection<string[]> AllValues { get; }
        ICollection<string[]> ValuesForNullKey { get; }
        ICollection<string[]> GetValuesForKey(string key);

        string FirstValueForNullKey { get; }
        string GetFirstValueForKey(string key);

        void Put(string id, ICollection<string[]> values);
    }
}