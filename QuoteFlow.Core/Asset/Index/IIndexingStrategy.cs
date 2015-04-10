using System;
using QuoteFlow.Api.Lucene.Index;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Core.Asset.Index
{
    public interface IIndexingStrategy : IDisposable
    {
        IIndexResult Get(ISupplier<IIndexResult> input);
    }
}