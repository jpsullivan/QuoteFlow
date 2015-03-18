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
        /// <returns>Reindex time in ms.</returns>
        int ReIndexAll();

        /// <summary>
        /// Reindex everything, but don't stop the world
        /// Comments will not be reindexed.
        /// </summary>
        /// <returns>Reindex time in ms.</returns>
        int ReIndexAllIssuesInBackground();

        /// <summary>
        /// Reindex everything, but don't stop the world
        /// </summary>
        /// <param name="reIndexComments"> Also reindex all the asset comments. </param>
        /// <returns>Reindex time in ms.</returns>
        int ReIndexAllIssuesInBackground(bool reIndexComments);

        /// <summary>
        /// Optimize the underlying indexes. Make the subsequent searching more efficient.
        /// </summary>
        /// <returns>The amount of time in millis this method took (because you are too lazy to time me), 0 if indexing is not enabled or -1 if we cannot obtain the index writeLock.</returns>
        int Optimize();

        /// <summary>
        /// Activates search indexes. This will rebuild the indexes.
        /// </summary>
        /// <returns>Reindex time in ms</returns>
        int Activate();

        /// <summary>
        /// Activates search indexes.
        /// </summary>
        /// <param name="reindex"> reindex after activation. </param>
        /// <returns> Reindex time in ms </returns>
        int Activate(bool reindex);

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
    }
}