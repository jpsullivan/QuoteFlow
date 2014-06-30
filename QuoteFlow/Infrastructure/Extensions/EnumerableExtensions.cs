﻿using System;
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

        public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> list, Func<T, TKey> lookup) where TKey : struct
        {
            return list.Distinct(new StructEqualityComparer<T, TKey>(lookup));
        }

        class StructEqualityComparer<T, TKey> : IEqualityComparer<T> where TKey : struct
        {
            readonly Func<T, TKey> _lookup;

            public StructEqualityComparer(Func<T, TKey> lookup)
            {
                _lookup = lookup;
            }

            public bool Equals(T x, T y)
            {
                return _lookup(x).Equals(_lookup(y));
            }

            public int GetHashCode(T obj)
            {
                return _lookup(obj).GetHashCode();
            }
        }
    }
}