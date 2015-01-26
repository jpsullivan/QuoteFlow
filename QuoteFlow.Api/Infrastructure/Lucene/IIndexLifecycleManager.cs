using System.Collections.Generic;
using QuoteFlow.Api.Infrastructure.Collect;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Api.Infrastructure.Lucene
{
    /// <summary>
    /// Manage an index lifecycle.
    /// </summary>
    public interface IIndexLifecycleManager : ISized, IShutdown
    {
        /// <summary>
        /// Reindex everything.
        /// </summary>
        /// <param name="context">Used to report progress back to the user or to the logs. Must not be null.</param>
        /// <returns>Reindex time in ms.</returns>
        int ReIndexAll(Job context);

        /// <summary>
        /// Reindex everything, but don't stop the world
        /// Comments and change history will not be reindexed.
        /// </summary>
        /// <param name="context">Used to report progress back to the user or to the logs. Must not be null.</param>
        /// <returns>Reindex time in ms.</returns>
        int ReIndexAllIssuesInBackground(Job context);

        /// <summary>
        /// Reindex everything, but don't stop the world
        /// </summary>
        /// <param name="context"> used to report progress back to the user or to the logs. Must not be null. </param>
        /// <param name="reIndexComments"> Also reindex all the issue comments. </param>
        /// <param name="reIndexChangeHistory"> Also reindex the issue change history. </param>
        /// <returns>Reindex time in ms.</returns>
        int ReIndexAllIssuesInBackground(Job context, bool reIndexComments, bool reIndexChangeHistory);

        /// <summary>
        /// Optimize the underlying indexes. Make the subsequent searching more efficient.
        /// </summary>
        /// <returns>The amount of time in millis this method took (because you are too lazy to time me), 0 if indexing is not enabled or -1 if we cannot obtain the index writeLock.</returns>
        int Optimize();

        /// <summary>
        /// Shuts down the indexing manager and closes its resources (if any).
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Activates search indexes. This will rebuild the indexes.
        /// </summary>
        /// <param name="context">Used to report progress back to the user or to the logs. Must not be null.</param>
        /// <returns>Reindex time in ms</returns>
        int Activate(Job context);

        /// <summary>
        /// Activates search indexes.
        /// </summary>
        /// <param name="context"> used to report progress back to the user or to the logs. Must not be null. </param>
        /// <param name="reindex"> reindex after activation. </param>
        /// <returns> Reindex time in ms </returns>
        int Activate(Job context, bool reindex);

        /// <summary>
        /// De-activates indexing (as happens from the admin page) and removes index directories.
        /// </summary>
        void Deactivate();

        /// <summary>
        /// Whether this index is available.
        /// The index is not available if the index is being rebuilt or recovered.
        /// In a clustered environment this reflects only the state on the local node.
        /// </summary>
        /// <returns>Whether this index is available.</returns>
        bool IndexAvailable { get; }

        /// <returns>
        /// The result of a simple consistency check that compares the index state to the 
        /// current number of issues.  A background re-index should not be attempted 
        /// when this returns {@code false}.</returns>
        bool IndexConsistent { get; }

        /// <returns> a collection of Strings that map to all paths that contain Lucene indexes. Must not be null. </returns>
        ICollection<string> AllIndexPaths { get; }

        /// <returns>
        /// How many Entities will be re-indexed by <seealso cref="ReIndexAll(Job)"/>
        /// </returns>
        int Size();
    }
}