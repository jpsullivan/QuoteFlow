using System;
using System.Collections.Generic;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Util;
using QuoteFlow.Api.Asset.Index;

namespace QuoteFlow.Core.Jql.Query
{
    /// <summary>
    /// Collect Asset Ids for subquery searchers.
    /// </summary>
    public class AssetIdCollector : Collector
    {
        /// <summary>
		/// This value is set to a large (conservative) value by trial and error.
		/// </summary>
		private const int SmallCollectRatio = 10000;

        private readonly int _maxDocs;
		private int _docBase;
        private readonly OpenBitSet _docIds;

		/// <summary>
		/// This is the reader of the whole index (not each segment).
		/// </summary>
		private readonly IndexReader _indexReader;
		private int _hitcount;
        private int _minDocId = int.MaxValue;
        private int _maxDocId = 0;

        public AssetIdCollector(IndexReader indexReader)
		{
			_indexReader = indexReader;
			_maxDocs = indexReader.MaxDoc;
			_docIds = new OpenBitSet(_maxDocs);
		}

        public override void SetScorer(Scorer scorer)
        {
            // do nothing
        }

        public override void Collect(int doc)
        {
            var index = _docBase + doc;
            _docIds.FastSet(index);
            _hitcount++;
            _minDocId = Math.Min(_minDocId, index);
            _maxDocId = Math.Max(_maxDocId, index);
        }

        public override void SetNextReader(IndexReader reader, int docBase)
        {
            _docBase = docBase;
        }

        public override bool AcceptsDocsOutOfOrder => true;

        public virtual ISet<string> AssetIds
        {
            get
            {
                int smallDocLimit = Math.Max(50, _maxDocs / SmallCollectRatio);

                if (_hitcount == 0)
                {
                    return new HashSet<string>();
                }

                if (_hitcount <= smallDocLimit)
                {
                    return AssetIdsDirectly;
                }

                return AssetIdsByTerms;
            }
        }

        public virtual ISet<string> AssetIdsDirectly
        {
            get
            {
                ISet<string> ids = new HashSet<string>();
                FieldSelector selector = new FieldSelectorResolver();

                // Do a virtual trim of the bit set so we don't unnecessarily scan to the end
                _docIds.NumWords = OpenBitSet.Bits2words(_maxDocId + 1);

                for (int docId = _docIds.NextSetBit(0); docId >= 0; docId = _docIds.NextSetBit(docId + 1))
                {
                    Document doc = _indexReader.Document(docId, selector);
                    ids.Add(doc.Get(DocumentConstants.AssetId));
                }

                return ids;
            }
        }

        private class FieldSelectorResolver : FieldSelector
        {
            public FieldSelectorResult Accept(string fieldName)
            {
                return fieldName.Equals(DocumentConstants.AssetId) ? FieldSelectorResult.LOAD_AND_BREAK : FieldSelectorResult.NO_LOAD;
            }
        }

        public virtual ISet<string> AssetIdsByTerms
        {
            get
            {
                ISet<string> issueIds = new HashSet<string>();

                TermDocs termDocs = _indexReader.TermDocs();
                TermEnum termEnum = _indexReader.Terms(new Term(DocumentConstants.AssetId, ""));
                try
                {
                    do
                    {
                        Term term = termEnum.Term;
                        // Lucene terms are interned so the != comparison is safe.
                        if (term == null || term.Field != DocumentConstants.AssetId)
                        {
                            // No comments. May happen
                            break;
                        }
                        string assetId = term.Text;

                        termDocs.Seek(termEnum);
                        while (termDocs.Next())
                        {
                            int doc = termDocs.Doc;

                            if (_docIds.Get(doc))
                            {
                                issueIds.Add(assetId);
                                break;
                            }
                        }
                    }
                    while (termEnum.Next());
                }
                finally
                {
                    termDocs.Dispose();
                    termEnum.Dispose();
                }
                return issueIds;
            }
        }

        public virtual ISet<string> AllAssetIds
        {
            get
            {
                ISet<string> assetIds = new HashSet<string>();

                TermEnum termEnum = _indexReader.Terms(new Term(DocumentConstants.AssetId, ""));
                try
                {
                    do
                    {
                        Term term = termEnum.Term;
                        // Lucene terms are interned so the != comparison is safe.
                        if (term == null || term.Field != DocumentConstants.AssetId)
                        {
                            // No comments. May happen
                            break;
                        }
                        string assetId = term.Text;
                        assetIds.Add(assetId);
                    }
                    while (termEnum.Next());
                }
                finally
                {
                    termEnum.Dispose();
                }
                return assetIds;
            }
        }
    }
}