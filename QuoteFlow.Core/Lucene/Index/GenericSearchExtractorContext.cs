using System;
using QuoteFlow.Api.Index;

namespace QuoteFlow.Core.Lucene.Index
{
    /// <summary>
    /// Simple context for searcher extractors.
    /// </summary>
    public class GenericSearchExtractorContext<T> : IEntitySearchExtractorContext<T>
    {
        public T Entity { get; private set; }
        public string IndexName { get; private set; }

        public GenericSearchExtractorContext(T entity, string indexName)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (indexName == null)
            {
                throw new ArgumentNullException(nameof(indexName));
            }

            Entity = entity;
            IndexName = indexName;
        }
    }
}
