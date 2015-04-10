using QuoteFlow.Api.Lucene.Index;
using QuoteFlow.Api.Util;
using QuoteFlow.Core.Asset.Index;

namespace QuoteFlow.Core.Index
{
    public class SimpleIndexingStrategy : IIndexingStrategy
    {
        public IIndexResult Get(ISupplier<IIndexResult> input)
        {
            return input.Get();
        }

        public void Dispose()
        {
        }
    }
}