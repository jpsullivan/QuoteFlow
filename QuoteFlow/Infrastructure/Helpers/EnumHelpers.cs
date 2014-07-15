using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace QuoteFlow.Infrastructure.Helpers
{
    public static class EnumHelpers
    {
        private static readonly ConcurrentDictionary<Type, IDictionary<object, string>> DescriptionMap = new ConcurrentDictionary<Type, IDictionary<object, string>>();

        public static string GetDescription<TEnum>(TEnum value) where TEnum : struct
        {
            Debug.Assert(typeof(TEnum).IsEnum); // Can't encode this in a generic constraint :(

            var descriptions = DescriptionMap.GetOrAdd(typeof(TEnum), key =>
                typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static).Select(f =>
                {
                    var v = f.GetValue(null);
                    var attr = f.GetCustomAttribute<DescriptionAttribute>();

                    string description = attr != null ? attr.Description : v.ToString();
                    return new KeyValuePair<object, string>(v, description);
                }).ToDictionary(p => p.Key, p => p.Value));

            string desc;
            if (descriptions == null || !descriptions.TryGetValue(value, out desc))
            {
                return value.ToString();
            }
            return desc;
        }
    }
}