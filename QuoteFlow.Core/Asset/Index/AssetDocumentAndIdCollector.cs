using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Core.Asset.Search;

namespace QuoteFlow.Core.Asset.Index
{
    /// <summary>
    /// Collects asset documents, IDs, and keys for use in an asset table.
    /// </summary>
    public class AssetDocumentAndIdCollector : Collector, ITotalHitsAwareCollector
    {
        public class Result
        {
            /// <summary>
            /// Returns the total number of issues matching the search.
            /// </summary>
            public int Total { get; set; }
            public int Start { get; set; }
            public IList<Document> Documents { get; set; }
            public IList<int?> AssetIds { get; set; }
            public IList<string> IssueKeys { get; set; }

            public Result(int start, int total, IList<int?> assetIds, IList<string> issueKeys, IList<Document> documents)
            {
                Start = start;
                Total = total;
                AssetIds = assetIds;
                IssueKeys = issueKeys;
                Documents = documents;
            }
        }

        private static readonly FieldSelector IdAndKeyFieldSelector = new SetBasedFieldSelector(
            new HashSet<string> { DocumentConstants.AssetId, DocumentConstants.AssetSku }, new HashSet<string>());

        private readonly IndexSearcher _searcher;
		private int _docBase;

		private readonly IList<int?> _docIds;
		private readonly int _maximumSize;
		private readonly int _pageSize;
		private readonly int? _selectedAssetId;
		private int _startIndex;
		private int _documentsCollected = 0;
		private int _totalHits = 0;

		/// <summary>
		/// 
		/// </summary>
        /// <param name="issuesSearcher"> </param>
        /// <param name="maximumSize">The maximum number of asset IDs / keys to collect (the stable search limit).</param>
        /// <param name="pageSize">The number of issues to be shown on a page of the asset table.</param>
        /// <param name="selectedAssetId">The ID of the selected asset (or {@code null}).</param>
        /// <param name="startIndex">The index of the first asset to show (ignored if <param name="selectedAssetId" /> is given).</param>
        public AssetDocumentAndIdCollector(IndexSearcher issuesSearcher, int maximumSize, int pageSize, int? selectedAssetId, int startIndex)
		{
			_searcher = issuesSearcher;
		    _docIds = new List<int?>(maximumSize);
			_maximumSize = maximumSize;
			_pageSize = pageSize;
			_selectedAssetId = selectedAssetId;
			_startIndex = NearestStartIndex(StartIndexWithinMaximumSize(startIndex, maximumSize), pageSize);
		}


        public override void SetScorer(Scorer scorer)
        {
            // do nothing
        }

        public override void Collect(int doc)
        {
            if (_documentsCollected < _maximumSize)
            {
                int docId = _docBase + doc;
                _docIds.Add(docId);
            }

            ++_documentsCollected;
        }

        public override void SetNextReader(IndexReader reader, int docBase)
        {
            _docBase = docBase;
        }

        public override bool AcceptsDocsOutOfOrder => false;

        public int TotalHits
        {
            set { _totalHits = value; }
        }

        private static int NearestStartIndex(int startIndex, int pageSize)
        {
            return (pageSize == 0) ? 0 : startIndex / pageSize * pageSize;
        }

        private static int StartIndexWithinMaximumSize(int startIndex, int maximumSize)
        {
            return (maximumSize == 0) ? 0 : Math.Min(startIndex, maximumSize - 1);
        }

        /// <summary>
        /// Computes the results for the currently matching page. Takes into consideration 
        /// the startIndex, selectedIssueKey, etc.
        /// </summary>
        public virtual Result ComputeResult()
        {
            try
            {
                int pageStartIdx = NearestStartIndex(StartIndexWithinMaximumSize(_startIndex, _docIds.Count), _pageSize);

                if (_selectedAssetId.HasValue)
                {
                    TermDocs docs = _searcher.IndexReader.TermDocs(new Term(DocumentConstants.AssetId, _selectedAssetId.Value.ToString()));
                    if (docs.Next())
                    {
                        int selectedDocId = docs.Doc;
                        int idx = _docIds.IndexOf(selectedDocId);
                        if (idx != -1)
                        {
                            // found selected issue, create page for that:
                            pageStartIdx = NearestStartIndex(idx, _pageSize);
                        }
                    }
                    docs.Dispose();
                }

                var assetIds = new List<int?>(_docIds.Count);
                var assetKeys = new List<string>(_docIds.Count);
                var documentsInPage = new List<Document>(Math.Min(_pageSize, _docIds.Count));

                for (int i = 0; i < pageStartIdx; i++)
                {
                    AddMatch(i, assetIds, assetKeys);
                }

                int pageEndIdx = Math.Min(pageStartIdx + _pageSize, _docIds.Count);
                for (int i = pageStartIdx; i < pageEndIdx; i++)
                {
                    AddMatch(i, assetIds, assetKeys, documentsInPage);
                }

                for (int i = pageEndIdx; i < _docIds.Count; i++)
                {
                    AddMatch(i, assetIds, assetKeys);
                }

                return new Result(pageStartIdx, _totalHits, assetIds, assetKeys, documentsInPage);
            }
            catch (IOException e)
            {
                throw new SearchException(e);
            }
        }

        private void AddMatch(int idx, IList<int?> assetIds, IList<string> assetKeys)
        {
            int? docId = _docIds[idx];
            Document doc = _searcher.Doc((int) docId, IdAndKeyFieldSelector);
            int? e = Convert.ToInt32(doc.Get(DocumentConstants.AssetId));
            assetIds.Add(e);
            assetKeys.Add(doc.Get(DocumentConstants.AssetSku));
        }

        private void AddMatch(int idx, IList<int?> assetIds, IList<string> assetKeys, IList<Document> documents)
        {
            int? docId = _docIds[idx];
            Document doc = _searcher.Doc((int) docId);
            assetIds.Add(Convert.ToInt32(doc.Get(DocumentConstants.AssetId)));
            assetKeys.Add(doc.Get(DocumentConstants.AssetSku));
            documents.Add(doc);
        }
    }
}
