using System;
using Lucene.Net.Store;

namespace QuoteFlow.Core.Lucene.Index
{
    public interface IDelayDisposable : IDisposable
    {
        /// <summary>
        /// Call this before usage and then call <see cref="close()"/> in the finally block.
        /// </summary>
        /// <exception cref="AlreadyClosedException"> if this has already been closed. </exception>
        void Open();

        /// <summary>
        /// Signals that this instance may really close when all open() calls have
        /// been balanced with a call to close().
        /// </summary>
        /// <exception cref="RuntimeIOException"> if an I/O error occurs. </exception>
        void CloseWhenDone();

        /// <summary>
        /// Returns whether the <see cref="IDisposable"/> has really been closed. If it is true, 
        /// this instance can no longer be used.
        /// </summary>
        /// <returns> whether the underlying <see cref="IDisposable"/> has really been closed.</returns>
        bool IsClosed { get; }
    }

    public sealed class DelayDisposableHelper : IDelayDisposable
    {
        // delegate?
        private readonly IDisposable _disposable;

        private readonly UsageTracker usageTracker = new UsageTracker();

        internal DelayDisposableHelper(IDisposable disposable)
        {
            _disposable = disposable;
        }

        /// <summary>
        /// This should be called whenever this instances is passed as a new
        /// IndexSearcher. Only when each call to open() is balanced with a call
        /// to close(), and closeWhenDone has been called, will super.close() be
        /// called.
        /// </summary>
        public void Open()
        {
            if (!usageTracker.IncrementUsage())
            {
                throw new AlreadyClosedException();
            }
        }

        public void CloseWhenDone()
        {
            if (!usageTracker.Close())
            {
                throw new AlreadyClosedException();
            }

            CheckClosed();
        }

        public void Dispose()
        {
            usageTracker.Decrement();
            CheckClosed();
        }

        /// <summary>
        /// Returns whether the underlying IndexSearcher has really been closed.
        /// If it is true, this instance can no longer be used.
        /// </summary>
        /// <returns>Whether the underlying IndexSearcher has really been closed.</returns>
        public bool IsClosed
        {
            get { return usageTracker.Closed; }
        }

        private void CheckClosed()
        {
            if (IsClosed)
            {
                _disposable.Dispose();
            }
        }

        /// <summary>
        /// Manage the state atomically. Cannot increment if closed.
        /// </summary>
        private sealed class UsageTracker
        {
            object obj = new object();

            /// <summary>
            /// The number of open() calls minus the number of close() calls. If
            /// this drops to zero and closeWhenDone() is true,
            /// <seealso cref="Closeable#close()"/> is called.
            /// </summary>
            private int _count;

            /// <summary>
            /// Indicates if closeWhenDone() was called. If true and usageCount
            /// is zero, super.close() is called.
            /// </summary>
            private bool _closed;

            internal bool IncrementUsage()
            {
                lock (obj)
                {
                    if (_closed)
                    {
                        return false;
                    }

                    _count++;
                    return true;
                }
            }

            internal void Decrement()
            {
                lock (obj)
                {
                    // don't decrement past zero
                    _count = (_count == 0) ? 0 : _count - 1;
                }
            }

            internal bool Close()
            {
                lock (obj)
                {
                    // check not true and then assign true
                    return !_closed && (_closed = true);
                }
            }

            internal bool Closed
            {
                get
                {
                    lock (obj)
                    {
                        return _closed && (_count == 0);
                    }
                }
            }
        }
    }
}