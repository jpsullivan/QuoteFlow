using System.Collections.Generic;
using Lucene.Net.Documents;

namespace QuoteFlow.Api.Asset.Index
{
    public interface IEntityDocumentBuilder<T>
    {
        Document Build(T entity);

        IEnumerable<Document> Build(IEnumerable<T> entities);
    }
}