using System;

namespace QuoteFlow.Core.Lucene.Index
{
    public interface IDisposableIndex : IIndex, IDisposable
    {
    }
}