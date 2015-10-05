using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Index;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Asset.Index;
using QuoteFlow.Core.Lucene.Index;

namespace QuoteFlow.Core.Index
{
    public class AssetIndexHelper
    {
        private readonly IAssetService _issueManager;
		private readonly IAssetIndexer _assetIndexer;

        public AssetIndexHelper(IAssetService issueManager, IAssetIndexer assetIndexer)
		{
			_issueManager = issueManager;
			_assetIndexer = assetIndexer;
		}

        /// <summary>
        /// Get all the asset ids known that are present in the index. 
        /// The asset ids are returned in a Sorted array.
        /// </summary>
        /// <returns>Array of asset ids.</returns>
        public int[] GetAllAssetIds()
        {
            return WithAssetSearcher(new AssetIdsHelper(this));
        }

        /// <summary>
        /// Takes care of opening/closing issueSearcher, and also wraps IOExceptions in
        /// runtime exceptions - everythin to remove all that pollution from function (in future
        /// try-with-resources may help)
        /// </summary>
        /// <param name="searcherFunc">Function that expects IndexSearcher.</param>
        /// <returns>What SearcherFunction returned.</returns> 
        private T WithAssetSearcher<T>(ISearcherFunction<T> searcherFunc)
        {
            try
            {
                IndexSearcher issueSearcher = _assetIndexer.OpenAssetSearcher();
                try
                {
                    T result = searcherFunc.Apply(issueSearcher);

                    return result;
                }
                finally
                {
                    issueSearcher.Dispose();
                }
            }
            catch (IOException x)
            {
                throw new Exception(x.ToString(), x.InnerException);
            }
        }

        /// <summary>
        /// Ensure the array has at least <see cref="requiredSize"/> elements.
        /// </summary>
        /// <param name="assetIds">Array to test.</param>
        /// <param name="requiredSize">Required size.</param>
        /// <returns></returns>
        public int[] EnsureCapacity(int[] assetIds, int requiredSize)
        {
            if (assetIds.Length < requiredSize)
            {
                // Expand the array.  This should occur rarely if ever so we only add a small increment
                int newSize = Math.Max(requiredSize, assetIds.Length + assetIds.Length / 10);
                Array.Resize(ref assetIds, newSize);
                return assetIds;
            }
            return assetIds;
        }

//        public virtual void FixupConcurrentlyIndexedAssets(Job context, AccumulatingResultBuilder resultBuilder, BackgroundIndexListener backgroundIndexListener, bool reIndexComments, bool reIndexChangeHistory)
//        {
//            // Safely reindex any asset that were concurrently updated - even if we have been cancelled.
//            AssetIdsAssetIterable issueIterable = new AssetIdsAssetIterable(backgroundIndexListener.UpdatedAssets, issueManager);
//
//            resultBuilder.add(issueIndexer.reindexIssues(issueIterable, context, reIndexComments, reIndexChangeHistory, true));
//            resultBuilder.toResult().@await();
//
//            // Make sure we haven't accidentally replaced any issues that were concurrently deleted.
//            safelyRemoveOrphans(resultBuilder, backgroundIndexListener.DeletedIssues);
//            resultBuilder.toResult().@await();
//        }
//
        public virtual void FixupIndexCorruptions(AccumulatingResultBuilder resultBuilder, IndexReconciler reconciler)
        {
            // Get issue that were found in the database but not in the index, They need to be reindexed again
            // if they still exist in the database and if they are still not in the index.
            // If they are in the database, then they have been indexed since we began the reindex and so all is well.
            SafelyAddMissing(resultBuilder, reconciler.Unindexed);
            resultBuilder.ToResult().Await();

            Debug.WriteLine("{0} missing assets added to the index.", reconciler.Unindexed.Count());

            // These issue were not found in the database but were in the index, They need to be removed
            // if they still do not exist in the database.
            SafelyRemoveOrphans(resultBuilder, reconciler.Orphans);
            resultBuilder.ToResult().Await();

            Debug.WriteLine("{0} deleted assets from the index.", reconciler.Orphans.Count());
        }

        public void SafelyAddMissing(AccumulatingResultBuilder resultBuilder, IEnumerable<long?> unindexed)
        {
            WithAssetSearcher(new SafelyAddMissingHelper(this, resultBuilder, unindexed));
        }

        private sealed class SafelyAddMissingHelper : ISearcherFunction<object>
        {
            private readonly AssetIndexHelper _outerInstance;

            private readonly AccumulatingResultBuilder _resultBuilder;
            private readonly IEnumerable<long?> _unindexed;

            public SafelyAddMissingHelper(AssetIndexHelper outerInstance, AccumulatingResultBuilder resultBuilder, IEnumerable<long?> unindexed)
            {
                _outerInstance = outerInstance;
                _resultBuilder = resultBuilder;
                _unindexed = unindexed;
            }

            public object Apply(IndexSearcher assetSearcher)
            {
                foreach (long? assetId in _unindexed)
                {
                    try
                    {
                        var asset = _outerInstance._issueManager.GetAsset((int) assetId);
                        if (asset == null) continue;

                        TermQuery query = new TermQuery(new Term(DocumentConstants.AssetId, Convert.ToString(assetId)));
                        TopDocs topDocs = assetSearcher.Search(query, 1);
                        if (topDocs.TotalHits == 0)
                        {
                            var assets = new List<Api.Models.Asset> { asset };
                            _resultBuilder.Add(_outerInstance._assetIndexer.ReIndexAssets(assets, true, false));
                        }
                    }
                    catch (IOException e)
                    {
                        _resultBuilder.Add(new Lucene.Index.Index.Failure(e));
                    }
                }

                return null;
            }
        }

        public void SafelyRemoveOrphans(AccumulatingResultBuilder resultBuilder, IEnumerable<int?> orphans)
        {
            WithAssetSearcher(new SafelyRemoveOrphansHelper(this, resultBuilder, orphans));
        }

        private sealed class SafelyRemoveOrphansHelper : ISearcherFunction<object>
        {
            private readonly AssetIndexHelper _outerInstance;

			private readonly AccumulatingResultBuilder _resultBuilder;
			private readonly IEnumerable<int?> _orphans;

			public SafelyRemoveOrphansHelper(AssetIndexHelper outerInstance, AccumulatingResultBuilder resultBuilder, IEnumerable<int?> orphans)
			{
				_outerInstance = outerInstance;
				_resultBuilder = resultBuilder;
				_orphans = orphans;
			}

			public object Apply(IndexSearcher assetSearcher)
			{
				foreach (long? assetId in _orphans)
				{
					try
					{
						var asset = _outerInstance._issueManager.GetAsset((int) assetId);
					    if (asset != null) continue;

					    TermQuery query = new TermQuery(new Term(DocumentConstants.AssetId, Convert.ToString(assetId)));
					    TopDocs topDocs = assetSearcher.Search(query, 1);
					    foreach (ScoreDoc scoreDoc in topDocs.ScoreDocs)
					    {
					        Document doc = assetSearcher.Doc(scoreDoc.Doc);
					        var assetToDelete = _outerInstance._issueManager.GetAsset(doc);
					        var assets = new List<IAsset> {assetToDelete};
					        _resultBuilder.Add(_outerInstance._assetIndexer.DeIndexAssets(assets));
					    }
					}
					catch (IOException e)
					{
						_resultBuilder.Add(new Lucene.Index.Index.Failure(e));
					}
				}
				return null;
			}

        }

        private class AssetIdsHelper : ISearcherFunction<int[]>
        {
            private AssetIndexHelper OuterInstance { get; set; }

            public AssetIdsHelper(AssetIndexHelper outerInstance)
            {
                OuterInstance = outerInstance;
            }

            public int[] Apply(IndexSearcher reader)
            {
                var indexReader = reader.IndexReader;
                // we know implicitly that there is exactly one and only one asset Id per document.
                var termEnum = indexReader.Terms(new Term(DocumentConstants.AssetId, ""));
                try
                {
                    int i = 0;
                    //var assetIds = new int[indexReader.NumDocs()];
                    var assetIds = new List<int>();
                    do
                    {
                        Term term = termEnum.Term;
                        // Lucene terms are interned so the != comparison is safe.
                        if (term == null || term.Field != DocumentConstants.AssetId)
                        {
                            // No assets. May happen
                            break;
                        }
                        string assetId = term.Text;
                        //assetIds = EnsureCapacity(assetIds, i);
                        //assetIds[i] = Convert.ToInt32(issueId);
                        assetIds.Add(Convert.ToInt32(assetId));
                        i++;
                    }
                    while (termEnum.Next());

                    return assetIds.ToArray();
                }
                finally
                {
                    termEnum.Dispose();
                }
            }
        }
    }
}