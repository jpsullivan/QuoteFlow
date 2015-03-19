using System;
using System.Collections.Generic;
using QuoteFlow.Api.Index;

namespace QuoteFlow.Core.Index
{
    public class SearchExtractorRegistrationManager : ISearchExtractorRegistrationManager
    {
        public IEnumerable<IEntitySearchExtractor<T>> FindExtractorsForEntity<T>(Type entityClass)
        {
            throw new NotImplementedException();
        }

        public void Register<T, T1>(IEntitySearchExtractor<T1> extractor, Type entityClass)
        {
            throw new NotImplementedException();
        }

        public void Unregister<T, T1>(IEntitySearchExtractor<T1> extractor, Type entityClass)
        {
            throw new NotImplementedException();
        }
    }
}