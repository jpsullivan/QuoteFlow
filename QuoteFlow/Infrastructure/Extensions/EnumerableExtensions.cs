using System;
using System.Collections.Generic;
using System.Linq;

namespace QuoteFlow.Infrastructure.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool AnySafe<T>(this IEnumerable<T> items)
        {
            return items != null && items.Any();
        }

        public static bool AnySafe<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            return items != null && items.Any(predicate);
        }
    }
}