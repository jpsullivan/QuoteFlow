using QuoteFlow.Api.Lucene.Index;
using QuoteFlow.Core.Lucene.Index;

namespace QuoteFlow.Core.Index
{
    /// <summary>
    /// Queueing <seealso cref="IIndex"/> implementation that takes all operations on the queue
    /// and batches them to the underlying <seealso cref="Index delegate"/> on the task thread.
    /// 
    /// The created thread is interruptible and dies when interrupted, but will be
    /// recreated if any new index jobs arrive. The initial task thread is not created
    /// until the first indexing job arrives.
    /// </summary>
    public class QueueingIndex : IDisposableIndex
    {
        public IIndexResult Perform(IndexOperation operation)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}