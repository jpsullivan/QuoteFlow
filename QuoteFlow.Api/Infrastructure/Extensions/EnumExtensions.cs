using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace QuoteFlow.Api.Infrastructure.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Retrieves the [Display(Name = "blah")] value from a decorated enum value.
        /// </summary>
        /// <param name="enumValue"></param>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static string GetDisplayAttributeFrom(this Enum enumValue, Type enumType)
        {
            return enumType.GetMember(enumValue.ToString())
                           .First()
                           .GetCustomAttribute<DisplayAttribute>()
                           .Name;
        }

        /// <summary>
        /// Get the value members from an enum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}