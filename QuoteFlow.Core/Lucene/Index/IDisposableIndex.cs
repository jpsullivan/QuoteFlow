using System;
using QuoteFlow.Api.Lucene.Index;

namespace QuoteFlow.Core.Lucene.Index
{
    public interface IDisposableIndex : IIndex, IDisposable
    {
    }
}