using Lucene.Net.Documents;

namespace QuoteFlow.Core.Lucene.Index
{
    /// <summary>
    /// Converts provided entity to lucene documents for indexing.
    /// </summary>
    public interface IEntityDocumentFactory<T>
    {
         
    }

    public abstract class EntityDocumentFactory<T>
    {
        protected readonly Document doc = new Document();
        private readonly GenericSearchExtractorContext<T> context;
    }
}