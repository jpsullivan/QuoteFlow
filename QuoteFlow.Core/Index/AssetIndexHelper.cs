using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Index;
using Lucene.Net.Search;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Index;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Asset.Index;

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
        /// Get all the asset ids known that are present in the index. The asset ids are returned in a Sorted array.
        /// </summary>
        /// <returns>Array of asset ids.</returns>
        public virtual int[] AllAssetIds
        {
            get { return WithAssetSearcher(new AssetIdsHelper(this)); }
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

        private static int[] EnsureCapacity(int[] assetIds, int i)
        {
            if (assetIds.Length <= i)
            {
                // Expand the array.  This should occur rarely if ever so we only add a small increment
                int newSize = Math.Max(i, assetIds.Length + assetIds.Length / 10);
                int[] assetIdsCopy = new int[newSize];

                assetIds.CopyTo(assetIdsCopy, newSize);
                return assetIdsCopy;
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
//        public virtual void FixupIndexCorruptions(AccumulatingResultBuilder resultBuilder, IndexReconciler reconciler)
//        {
//            // Get issue that were found in the database but not in the index, They need to be reindexed again
//            // if they still exist in the database and if they are still not in the index.
//            // If they are in the database, then they have been indexed since we began the reindex and so all is well.
//            safelyAddMissing(resultBuilder, reconciler.Unindexed);
//            resultBuilder.toResult().@await();
//
//            log.debug("" + reconciler.Unindexed.size() + " missing issues add to the index.");
//
//            // These issue were not found in the database but were in the index, They need to be removed
//            // if they still do not exist in the database.
//            safelyRemoveOrphans(resultBuilder, reconciler.Orphans);
//            resultBuilder.toResult().@await();
//
//            log.debug("" + reconciler.Orphans.size() + " deleted issues removed from the index.");
//        }

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