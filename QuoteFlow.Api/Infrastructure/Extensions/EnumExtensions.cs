using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        /// An alternative to <see cref="GetDisplayAttributeFrom"/>, this will return the
        /// string value of whatever is defined within the [Display(Name = "blah")] attribute.
        /// </summary>
        /// <param name="enumeration"></param>
        /// <returns></returns>
        public static string ToDescription(this Enum enumeration)
        {
            string name = enumeration.ToString();
            DescriptionAttribute[] descriptionAttributeArray = enumeration.GetType().GetField(name).GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            if (descriptionAttributeArray == null || descriptionAttributeArray.Length == 0)
                return string.Empty;
            return descriptionAttributeArray[0].Description;
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