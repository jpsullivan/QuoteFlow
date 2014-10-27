using System;
using System.Collections.Generic;
using System.Linq;
using Wintellect.PowerCollections;

namespace QuoteFlow.Models.Assets.Transport
{
    public class ActionParams : IActionParams
    {
        protected internal MultiDictionary<string, String[]> actionParams;


        public ActionParams()
        {
            actionParams = new MultiDictionary<string, String[]>(true);
        }

        public ActionParams(MultiDictionary<string, String[]> actionParams)
        {
            this.actionParams = actionParams;
        }

        public ICollection<string> AllKeys
        {
            get { return actionParams.Keys; }
        }

        public MultiDictionary<string, String[]> KeysAndValues
        {
            get { return actionParams; }
        }

        public bool ContainsKey(string key)
        {
            return actionParams.ContainsKey(key);
        }

        public bool IsEmpty()
        {
            return actionParams.Count == 0;
        }

        public ICollection<string[]> AllValues
        {
            get { return actionParams.Values; }
        }

        public ICollection<string[]> ValuesForNullKey
        {
            get { return GetValuesForKey(null); }
        }

        public ICollection<string[]> GetValuesForKey(string key)
        {
            return actionParams[key];
        }

        public string FirstValueForNullKey
        {
            get { return GetFirstValueForKey(null); }
        }

        public string GetFirstValueForKey(string key)
        {
            var c = GetValuesForKey(key);
            return c.Any() ? c.First()[0] : null;
        }

        public void Put(string id, ICollection<string[]> values)
        {
            actionParams[id] = values;
        }
    }
}