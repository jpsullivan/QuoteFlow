using System;
using System.Collections.Generic;
using Lucene.Net.Search;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Lucene.Index;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Asset.Index
{
    public interface IAssetIndexer
    {
        /// <summary>
        /// Add documents for the supplied assets.
        /// </summary>
        /// <param name="assets">A collection of assets to index.</param>
        IIndexResult IndexAssets(IEnumerable<IAsset> assets);

        /// <summary>
        /// Add documents for the supplied assets.
        /// </summary>
        /// <param name="assets">A collection of assets to index.</param>
        /// <param name="assetIndexingParams">Parameters describing what should be reindexed.</param>
        IIndexResult IndexAssets(IEnumerable<IAsset> assets, AssetIndexingParams assetIndexingParams);

        /// <summary>
        /// Delete any existing documents for the supplied assets.
        /// </summary>
        /// <param name="assets">An iterable of assets to index.</param>
        IIndexResult DeIndexAssets(IEnumerable<IAsset> assets);

        /// <summary>
        /// Re-index the given assets, delete any existing documents and add new ones.
        /// </summary>
        /// <param name="assets">An interable of assets to reindex.</param>
        /// <param name="reIndexComments">Set to true if you require the asset comments to also be reindexed.</param>
        /// <param name="conditionalUpdate">Set to true to use conditional updates when writing to the index.</param>
        [Obsolete]
        IIndexResult ReIndexAssets(IEnumerable<Api.Models.Asset> assets, bool reIndexComments, bool conditionalUpdate);

        /// <summary>
        /// Re-index the given assets, delete any existing documents and add new ones.
        /// </summary>
        /// <param name="assets">An iterable of assets to index.</param>
        /// <param name="assetIndexingParams">Parameters for describing what should be reindexed.</param>
        /// <param name="conditionalUpdate">Set to true to use conditional updates when writing to the index.</param>
        IIndexResult ReIndexAssets(IEnumerable<Api.Models.Asset> assets, AssetIndexingParams assetIndexingParams, bool conditionalUpdate);

        /// <summary>
        /// Reindex a collection of asset comments. 
        /// </summary>
        /// <param name="comments">Comments to be reindexed.</param>
        IIndexResult ReIndexComments(ICollection<AssetComment> comments);

        /// <summary>
        /// Index the given assets, use whatever is in your arsenal to do it as FAST as possible.
        /// </summary>
        /// <param name="assets">An iterable of assets to index.</param>
        IIndexResult IndexAssetsBatchMode(IEnumerable<IAsset> assets);

        IIndexResult Optimize();

        /// <summary>
        /// Deletes all indexes.
        /// </summary>
        void DeleteIndexes();

        /// <summary>
        /// Delete selected indexes. 
        /// </summary>
        /// <param name="assetIndexingParams"></param>
        void DeleteIndexes(AssetIndexingParams assetIndexingParams);

        void Shutdown();

        /// <summary>
        /// Asset searcher has to be closed after doing stuff.
        /// </summary>
        IndexSearcher OpenAssetSearcher();

        /// <summary>
        /// Comment searcher has to be closed after doing stuff.
        /// </summary>
        /// <returns></returns>
        IndexSearcher OpenCommentSearcher();

        IList<string> IndexPaths { get; }

        string IndexRootPath { get; }
    }

    public class AssetIndexerAnalyzers
    {
        public static readonly global::Lucene.Net.Analysis.Analyzer Searching = QuoteFlowAnalyzer.AnalyzerForSearching;
        public static readonly global::Lucene.Net.Analysis.Analyzer Indexing = QuoteFlowAnalyzer.AnalyzerForIndexing;
    }
}