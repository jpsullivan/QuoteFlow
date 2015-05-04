using System;

namespace QuoteFlow.Api.Lucene.Index
{
    /// <summary>
    /// The payload is unimportant. Call <see cref="#await()"/> simply to block on the
    /// result being computed.
    /// </summary>
    public interface IIndexResult
    {
        /// <summary>
        /// Await the result of the operation.
        /// </summary>
        void Await();

        /// <summary>
        /// Await the result of the operation for the specified time, throwing a
        /// <see cref="TimeoutException"/> if the timeout is reached.
        /// </summary>
        /// <param name="timeout"> the amount to wait </param>
        /// <param name="unit">The unit to count the timeout in.</param>
        /// <returns>False if the timeout is exceeded before the underlying operation has completed, true if it has completed in time.</returns>
        //bool Await(long timeout, TimeSpan unit);

        /// <summary>
        /// Has the operation completed yet. If true then <see cref="Await()"/> and
        /// <see cref="Await(long, TimeSpan)"/> will not block.
        /// </summary>
        /// <returns> whether the operation is complete or not. </returns>
        bool Done { get; }
    }
}
