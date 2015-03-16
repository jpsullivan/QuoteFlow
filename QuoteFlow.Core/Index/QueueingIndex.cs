using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using QuoteFlow.Api.Lucene.Index;
using QuoteFlow.Core.Lucene.Index;

namespace QuoteFlow.Core.Index
{
    /// <summary>
    /// Queueing <see cref="IIndex"/> implementation that takes all operations on the queue
    /// and batches them to the underlying <see cref="Index delegate"/> on the task thread.
    /// 
    /// The created thread is interruptible and dies when interrupted, but will be
    /// recreated if any new index jobs arrive. The initial task thread is not created
    /// until the first indexing job arrives.
    /// </summary>
    public class QueueingIndex : IDisposableIndex
    {
        private static readonly BlockingCollection<FutureOperation> queue = new BlockingCollection<FutureOperation>();
        private Task<BlockingCollection<FutureOperation>> task = new Task<BlockingCollection<FutureOperation>>(() => queue);

        public QueueingIndex(string name, Lucene.Index.Index index, long maxQueueSize)
        {
            throw new NotImplementedException();
        }

        public IIndexResult Perform(Operation operation)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Class that is responsible for returning <see cref="IIndexResult"/> to the calling thread.
        /// Calls to <see cref="FutureOperation.Get()"/> will block until the reference is set.
        /// </summary>
        internal class FutureOperation : TaskCompletionSource<IIndexResult>
        {
            private readonly Operation _operation;

            internal FutureOperation(Operation operation)
            {
                if (operation == null) throw new ArgumentNullException("operation");

                _operation = operation;
            }

            internal virtual UpdateMode Mode()
            {
                return _operation.Mode();
            }
        }

//        internal class CompositeOperation : IndexOperation
//		{
//            private readonly IEnumerable<FutureOperation> _operations;
//
//			internal CompositeOperation(IEnumerable<FutureOperation> operations)
//			{
//				_operations = operations;
//			}
//
//			public virtual void Set(IIndexResult result)
//			{
//				foreach (FutureOperation future in _operations)
//				{
//					future.Set(result);
//				}
//			}
//
//			public override void Perform(IWriter writer)
//			{
//				IEnumerator<FutureOperation> iter = _operations.GetEnumerator();
//				try
//				{
//					while (iter.MoveNext())
//					{
//						iter.Current.operation.perform(writer);
//					}
//				}
//				catch (IOException ioe)
//				{
//					CancelTheRest(iter, ioe);
//					throw ioe;
//				}
//                catch (Exception re)
//				{
//					CancelTheRest(iter, re);
//					throw re;
//				}
//			}
//
//            private static void CancelTheRest(IEnumerator<FutureOperation> iter, Exception cause)
//			{
//				var ce = new CancellationException("Cancelled composite indexing operation due to unhandled exception " + cause);
//				ce.InitCause(cause);
//				while (iter.MoveNext())
//				{
//					iter.Current.Exception = ce;
//				}
//			}
//
//            public override UpdateMode Mode
//            {
//                get
//                {
//                    //@TODO check size to simply return BATCH
//                    return _operations.Any(future => future.Mode() == UpdateMode.Batch) ? UpdateMode.Batch : UpdateMode.Interactive;
//                }
//                set { throw new NotImplementedException(); }
//            }
//		}
    }
}