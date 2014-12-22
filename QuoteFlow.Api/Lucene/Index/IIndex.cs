using System;
using Lucene.Net.Search;

namespace QuoteFlow.Api.Lucene.Index
{
    /// <summary>
    /// An <seealso cref="Index"/> is where data is stored for fast retrieval. The
    /// <seealso cref="Index"/> itself has <seealso cref="Operation operations"/> performed on it to
    /// update it. The index is held by a <seealso cref="Manager"/> where you can access a
    /// <seealso cref="IndexSearcher searcher"/> that reflects the latest update that has
    /// completed.
    /// 
    /// Note: in order to guarantee that an <seealso cref="IndexSearcher"/> returned from
    /// <seealso cref="Index.Manager#openSearcher()"/> contains a particular <seealso cref="IOperation"/>
    /// that is <seealso cref="Perform(Operation) performed"/>, the <seealso cref="Result"/> must be
    /// <seealso cref="IIndex.Result.Await() waited on"/>.
    /// </summary>
    public interface IIndex
    {
        /// <summary>
        /// Perform an <seealso cref="Operation"/> on the index.
        /// </summary>
        /// <param name="operation"> the work to do. </param>
        /// <returns> a Result object </returns>
        IIndexResult Perform(IndexOperation operation);
    }

    /// <summary>
    /// The payload is unimportant. Call <seealso cref="#await()"/> simply to block on the
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
        /// <seealso cref="TimeoutException"/> if the timeout is reached.
        /// </summary>
        /// <param name="timeout"> the amount to wait </param>
        /// <param name="unit">The unit to count the timeout in.</param>
        /// <returns>False if the timeout is exceeded before the underlying operation has completed, true if it has completed in time.</returns>
        bool Await(long timeout, TimeSpan unit);

        /// <summary>
        /// Has the operation completed yet. If true then <seealso cref="Await()"/> and
        /// <seealso cref="Await(long, TimeSpan)"/> will not block.
        /// </summary>
        /// <returns> whether the operation is complete or not. </returns>
        bool Done { get; }
    }

    /// <summary>
    /// Management of an <seealso cref="Index"/>
    /// </summary>
    public interface IIndexManager : IDisposable
    {
        /// <summary>
        /// Get the current IndexConnection this manager holds. May throw
        /// exceptions if the index has not been created.
        /// </summary>
        /// <returns>The Index this manager refers to.</returns>
        IIndex Index { get; }

        /// <summary>
        /// Get the current <seealso cref="IndexSearcher"/> from the <seealso cref="Index"/>.
        /// 
        /// You must call the <seealso cref="IndexSearcher#close() close"/> method in a
        /// finally block once the searcher is no longer needed.
        /// </summary>
        /// <returns> the current <seealso cref="IndexSearcher"/></returns>
        IndexSearcher OpenSearcher();

        /// <summary>
        /// Returns true if the index has been created. This means that the index
        /// directory itself exists AND has been initialised with the default
        /// required index files.
        /// </summary>
        /// <returns> true if the index exists, false otherwise. </returns>
        bool IndexCreated { get; }

        /// <summary>
        /// Clean out the underlying directory the index is contained in.
        /// Blow away any indexes that currently live there.
        /// </summary>
        void DeleteIndexDirectory();
    }

    public static class IndexUpdateMode
    {
        public static 
    }

}