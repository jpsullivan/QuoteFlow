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

        public string[] AllValues
        {
            get
            {
                var allValues = new List<string>();
                foreach (var array in actionParams.Values)
                {
                    allValues.AddRange(array);
                }

                return allValues.ToArray();
            }
        }

        public string[] ValuesForNullKey
        {
            get { return GetValuesForKey(null); }
        }

        public string[] GetValuesForKey(string key)
        {
            var values = new List<string>();
            foreach (var actionParam in actionParams)
            {
                if (actionParam.Key == key)
                {
                    
                    foreach (var value in actionParam.Value)
                    {
                        values.Add(value[0]);
                    }
                }
            }

            return values.Any() ? values.ToArray() : null;

            //return values.ToArray();

            //return actionParams.Where(ap => ap.Key == key).Select(ap => ap.Value.First()).FirstOrDefault();
        }

        public string FirstValueForNullKey
        {
            get { return GetFirstValueForKey(null); }
        }

        public string GetFirstValueForKey(string key)
        {
            var c = GetValuesForKey(key);
            if (c == null) return null;
            return c.Any() ? c[0] : null;
        }

        public void Put(string id, string[] values)
        {
            actionParams[id] = new[] {values};
        }
    }
}