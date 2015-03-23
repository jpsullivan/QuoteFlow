using System;
using System.Collections;
using System.Collections.Generic;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util.Support;

namespace QuoteFlow.Core.Asset.Index
{
    /// <summary>
    /// A helper when doing backgound re-indexing, to match up which assets are seen
    /// in the index and the database.
    /// </summary>
    public class IndexReconciler : AssetIdBatcher.ISpy
    {
        public virtual IList<long?> Unindexed { get; private set; }

        private readonly int[] _indexedIssues;
        private readonly BitArray _matched;

        /// <summary>
        /// Construct a new Reconciler. </summary>
        /// <param name="indexedIssues"> An array of issues known to be in the index. </param>
        public IndexReconciler(int[] indexedIssues)
        {
            Unindexed = new List<long?>();
            _indexedIssues = indexedIssues;
            Array.Sort(_indexedIssues);
            _matched = new BitArray(indexedIssues.Length);
        }

        public void Spy(IAsset asset)
        {
            // As we see each issue mark it in the bit set as matched.
            int i = Array.BinarySearch(_indexedIssues, asset.Id);
            if (i >= 0)
            {
                _matched.Set(i, true);
            }
            else
            {
                Unindexed.Add(asset.Id);
            }
        }

        protected virtual IEnumerable<int?> Orphans
        {
            get
            {
                var orphans = new List<int?>();
                for (int i = _matched.NextClearBit(0); i >= 0; i = _matched.NextClearBit(i + 1))
                {
                    if (i >= _indexedIssues.Length)
                    {
                        break;
                    }
                    orphans.Add(_indexedIssues[i]);
                }
                return orphans;
            }
        }
    }
}