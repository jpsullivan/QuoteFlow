using System;
using Lucene.Net.Search;
using QuoteFlow.Core.Index;
using QuoteFlow.Core.Lucene.Index;

namespace QuoteFlow.Core.Tests.Index
{
    public class MockIndexEngine : IEngine
    {
        public Action OnDispose { get; set; } 
        public Action<Operation> OnWrite { get; set; } 

        public void Dispose()
        {
            OnDispose?.Invoke();
        }

        public void Write(Operation operation)
        {
            OnWrite?.Invoke(operation);
        }

        public IndexSearcher GetSearcher()
        {
            throw new NotImplementedException();
        }

        public void Clean()
        {
            throw new NotImplementedException();
        }
    }
}