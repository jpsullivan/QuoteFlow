using System.Collections.Generic;
using Lucene.Net.Search;
using QuoteFlow.Api.Lucene.Index;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Asset.Index
{
    public interface IAssetIndexer
    {
        /// <summary>
        /// Add documents for the supplied assets.
        /// </summary>
        /// <param name="assets"> An iterable of assets to index. </param>
        /// <param name="context"> for showing the user the current status. </param>
        IIndexResult IndexAssets(IEnumerable<IAsset> assets, Job context);

        /// <summary>
        /// Delete any existing documents for the supplied assets.
        /// </summary>
        /// <param name="assets"> An iterable of assets to index. </param>
        /// <param name="context"> for showing the user the current status. </param>
        IIndexResult DeIndexAssets(IEnumerable<IAsset> assets, Job context);

        /// <summary>
        /// Re-index the given assets, delete any existing documents and add new ones.
        /// </summary>
        /// <param name="assets"> An iterable of assets to index. </param>
        /// <param name="context"> for showing the user the current status. </param>
        /// <param name="reIndexComments"> Set to true if you require asset comments to also be reindexed. </param>
        /// <param name="reIndexChangeHistory"> Set to true if you require asset change history to also be reindexed. </param>
        /// <param name="conditionalUpdate"> set to true to use conditional updates when writing to the index </param>
        IIndexResult ReIndexAssets(IEnumerable<IAsset> assets, Job context, bool reIndexComments, bool reIndexChangeHistory, bool conditionalUpdate);

        /// <summary>
        /// Reindex a collection of asset comments. 
        /// </summary>
        /// <param name="comments"> Comments to be reindexed. </param>
        /// <param name="context"> for showing the user the current status. </param>
        IIndexResult ReIndexComments(ICollection<AssetComment> comments, Job context);

        /// <summary>
        /// Index the given assets, use whatever is in your arsenal to do it as FAST as possible.
        /// </summary>
        /// <param name="assets"> An iterable of assets to index. </param>
        /// <param name="context"> for showing the user the current status. </param>
        IIndexResult IndexAssetsBatchMode(IEnumerable<IAsset> assets, Job context);

        IIndexResult Optimize();

        void DeleteIndexes();

        void Shutdown();

        /// <summary>
        /// asset searcher has to be closed after doing stuff.
        /// </summary>
        IndexSearcher OpenAssetSearcher();

        /*
         * asset searcher has to be closed after doing stuff.
         */
        IndexSearcher OpenCommentSearcher();

        /// <summary>
        /// asset searcher has to be closed after doing stuff.
        /// </summary>
        IndexSearcher OpenChangeHistorySearcher();

        IList<string> IndexPaths { get; }

        string IndexRootPath { get; }
    }

    public class AssetIndexerAnalyzers
    {
        public static readonly global::Lucene.Net.Analysis.Analyzer Searching = QuoteFlowAnalyzer.AnalyzerForSearching;
        public static readonly global::Lucene.Net.Analysis.Analyzer Indexing = QuoteFlowAnalyzer.AnalyzerForIndexing;
    }
}