using System.Collections.Generic;
using System.Threading;
using Lucene.Net.Search;
using QuoteFlow.Api.Infrastructure.Lucene;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Asset.Index
{
    /// <summary>
    /// Manages Lucene search indexes.
    /// </summary>
    public interface IAssetIndexManager : IIndexLifecycleManager
    {
        /// <summary>
        /// Reindex all issues.
        /// </summary>
        /// <returns> Reindex time in ms. </returns>
        int ReIndexAll();

        /// <summary>
        /// Reindex all assets.
        /// </summary>
        /// <param name="useBackgroundReindexing">Whether to index in the background or not. If the useBackgroundReindexing option is
        /// set to true, then comments and change history will not be reindexed.</param>
        /// <param name="updateReplicatedIndexStore"> whether to update the replicated index or not </param>
        /// <returns>Reindex time in ms.</returns>
        int ReIndexAll(bool useBackgroundReindexing, bool updateReplicatedIndexStore);

        /// <summary>
        /// Reindex all assets.
        /// If the useBackgroundReindexing option is set to true, then only the basic issue information will be reindexed, unless the
        /// ReIndexComments or ReIndexChangeHistory parameters are also set.
        /// This is considered the normal mode for background re-indexing and is sufficient to correct the index for changes in the
        /// system configuration, but not for changes to the indexing language.
        /// If useBackgroundReindexing is set to false, than everything is always reindexed.
        /// </summary>
        /// <param name="useBackgroundReindexing">Whether to index in the background or not.</param>
        /// <param name="reIndexComments">Also reindex all the issue comments. Only relevant for background reindex operations.</param>
        /// <param name="updateReplicatedIndexStore">Whether to update the replicated index or not.</param>
        /// <returns>Reindex time in ms.</returns>
        int ReIndexAll(bool useBackgroundReindexing, bool reIndexComments, bool updateReplicatedIndexStore);

        /// <summary>
        /// Reindex a list of assets, passing an optional event that will be set progress
        /// </summary>
        /// <param name="assets">The assets to enumerate.</param>
        /// <returns>Reindex time in ms.</returns>
        int ReIndexAssets(IEnumerable<IAsset> assets);

        /// <summary>
        /// Reindex a list of assets, passing an optional event that will be set progress. This method can optionally also
        /// index the comments.
        /// </summary>
        /// <param name="assets">The assets to enumerate.</param>
        /// <param name="reIndexComments">A boolean indicating whether to index asset comments.</param>
        /// <returns>Reindex time in ms.</returns>
        int ReIndexAssets(IEnumerable<IAsset> assets, bool reIndexComments);

        /// <summary>
        /// Reindex an asset (eg. after field updates).
        /// </summary>
        void ReIndex(IAsset issue);

        /// <summary>
        /// Reindex an asset (eg. after field updates).
        /// </summary>
        void ReIndex(IAsset issue, bool reIndexComments);

        /// <summary>
        /// Reindexes a collection of comments.
        /// </summary>
        /// <param name="comments">A collection of Comment</param>
        int ReIndexComments(IEnumerable<AssetComment> comments);

        /// <summary>
        /// Reindexes a collection of comments.
        /// </summary>
        /// <param name="comments">A collection of asset comments.</param>
        int ReIndexComments(ICollection<AssetComment> comments);


        /// <summary>
        /// Reindexes a collection of comments.
        /// </summary>
        /// <param name="comments">A collection of asset comments.</param>
        /// <param name="updateReplicatedIndexStore">Whether to update the replicated index or not.</param>
        int ReIndexComments(ICollection<AssetComment> comments, bool updateReplicatedIndexStore);

        /// <summary>
        /// Remove an asset from the search index.
        /// </summary>
        void DeIndex(IAsset asset);

        /// <summary>
        /// Remove a set of assets from the search index.
        /// </summary>
        void DeIndexAssetObjects(ISet<IAsset> issuesToDelete, bool updateReplicatedIndexStore);

        /// <summary>
        /// Reindex a set of assets.
        /// </summary>
        /// <param name="issueObjects"> Set of <seealso cref="IAsset"/>s to reindex.</param>
        /// <returns>Reindex time in ms.</returns>
        int ReIndexAssetObjects<T>(ICollection<T> issueObjects) where T : IAsset;

        /// <summary>
        /// Reindex a set of assets.
        /// </summary>
        /// <param name="issueObjects">Set of <seealso cref="IAsset"/>s to reindex.</param>
        /// <param name="reIndexComments"></param>
        /// <returns>Reindex time in ms.</returns>
        int ReIndexAssetObjects<T>(ICollection<T> issueObjects, bool reIndexComments) where T : IAsset;

        /// <summary>
        /// Reindex a set of issues.
        /// </summary>
        /// <param name="assetObjects">Set of <seealso cref="IAsset"/>s to reindex.</param>
        /// <param name="reIndexComments"> whether to reindex the comments or not.</param>
        /// <param name="updateReplicatedIndexStore">Whether to store index operations in the replicated index store.</param>
        /// <returns>Reindex time in ms.</returns>
        long ReIndexAssetObjects<T>(ICollection<T> assetObjects, bool reIndexComments, bool updateReplicatedIndexStore) where T : IAsset;

        /// <summary>
        /// Temporarily suspend indexing on this thread.  All index requests will be queued and processed
        /// when release is called.
        /// </summary>
        void Hold();

        /// <summary>
        /// Return true if the index is held.
        /// </summary>
        bool Held { get; }

        /// <summary>
        /// Release indexing on this thread.  All queued index requests will be processed. </summary>
        /// <returns> Reindex time in ms. </returns>
        long Release();

        /// <summary>
        /// Get an <seealso cref="IndexSearcher"/> that can be used to search the issue index.
        /// <p />
        /// Note: This is an unmanaged IndexSearcher. You MUST call <seealso cref="IndexSearcher#close()"/> when you are done with it. Alternatively you should
        /// really call <seealso cref="SearchProviderFactory.GetSearcher(string)"/> passing in <seealso cref="SearchProviderFactory.ISSUE_INDEX"/> as it is a managed searcher
        /// and all the closing semantics are handled for you.
        /// </summary>
        IndexSearcher AssetSearcher { get; }

        /// <summary>
        /// Get an <seealso cref="IndexSearcher"/> that can be used to search the comment index.
        /// <p />
        /// Note: This is an unmanaged IndexSearcher. You MUST call <seealso cref="IndexSearcher#close()"/> when you are done with it. Alternatively you should
        /// really call <seealso cref="SearchProviderFactory#getSearcher(String))"/> passing in <seealso cref="SearchProviderFactory#COMMENT_INDEX"/> as it is a managed
        /// searcher and all the closing semantics are handled for you.
        /// </summary>
        IndexSearcher CommentSearcher { get; }

        /// <summary>
        /// Returns an <seealso cref="Analyzer"/> for searching.
        /// </summary>
        /// <returns>An analyzer for searching.</returns>
        global::Lucene.Net.Analysis.Analyzer AnalyzerForSearching { get; }

        /// <summary>
        /// Returns an <seealso cref="Analyzer"/> for indexing.
        /// </summary>
        /// <returns>An analyzer for indexing.</returns>
        global::Lucene.Net.Analysis.Analyzer AnalyzerForIndexing { get; }

        /// <summary>
        /// Runs the given runnable under the 'stop the world' reindex lock.
        /// </summary>
        /// <param name="runnable">The runnable thread to be executed.</param>
        /// <returns>True if the lock could be acquired, false otherwise</returns>
        bool WithReindexLock(ThreadStart runnable);
    }
}