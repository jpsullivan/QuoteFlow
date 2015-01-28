using System.Collections.Concurrent;

namespace QuoteFlow.Api.Infrastructure.Extensions
{
    public static class DictionaryExtensions
    {
        public static void AddOrSet<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> self, TKey key, TValue val)
        {
            self.AddOrUpdate(key, val, (_, __) => val);
        }
    }
}
