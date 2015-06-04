using System;
using System.Runtime.Caching;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Asset.Search;

namespace QuoteFlow.Core.Tests.Caching
{
    /// <summary>
    /// A simple memory cache implementation to get past the 
    /// HttpContext caching layer that <see cref="QueryCache"/> uses.
    /// </summary>
    public class MemoryCacheService : ICacheService
    {
        private readonly ObjectCache _cache = MemoryCache.Default;

        public object GetItem(string key)
        {
            return _cache.Get(key);
        }

        public void SetItem(string key, object item, TimeSpan timeout)
        {
            _cache.Set(key, item, DateTimeOffset.MaxValue);
        }

        public void RemoveItem(string key)
        {
            _cache.Remove(key);
        }
    }
}