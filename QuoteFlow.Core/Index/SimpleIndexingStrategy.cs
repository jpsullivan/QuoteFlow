using QuoteFlow.Api.Lucene.Index;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Core.Index
{
    public class SimpleIndexingStrategy
    {
        public IIndexResult Get(ISupplier<IIndexResult> input)
        {
            return input.Get();
        }

        public void Close()
        {
        }
    }
}