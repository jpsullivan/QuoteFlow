using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

    public static class EnumHelpers<T>
    {
        public static IList<T> GetValues(Enum value)
        {
            var enumValues = new List<T>();

            foreach (FieldInfo fi in value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                enumValues.Add((T)Enum.Parse(value.GetType(), fi.Name, false));
            }
            return enumValues;
        }

        public static T Parse(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static IEnumerable<string> GetNames(Enum value)
        {
            return value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name).ToList();
        }

        public static IList<string> GetDisplayValues(Enum value)
        {
            return GetNames(value).Select(obj => GetDisplayValue(Parse(obj))).ToList();
        }

        public static string GetDisplayValue(T value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(
                typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (descriptionAttributes == null) return string.Empty;
            return (descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : value.ToString();
        }
    }
}