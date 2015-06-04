using QuoteFlow.Core.Asset.Search;

namespace QuoteFlow.Core.Tests.Caching
{
    public class MockQueryCache : QueryCache
    {
        protected MockQueryCache()
        {
            CacheService = new MemoryCacheService();
        }
    }
}