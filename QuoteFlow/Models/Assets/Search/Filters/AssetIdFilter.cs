using System.Collections.Generic;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Util;
using QuoteFlow.Models.Assets.Index;

namespace QuoteFlow.Models.Assets.Search.Filters
{
    /// <summary>
    /// This filter will return only the list of issues that match the issue Ids passed in.
    /// This is useful for queries that query other data sources, before being combined with an asset search 
    /// (eg comment or change history). 
    /// </summary>
    public class AssetIdFilter : Filter
    {
        private readonly ISet<string> _assetIds;

        public AssetIdFilter(ISet<string> assetIds)
        {
            _assetIds = assetIds;
        }

        public override DocIdSet GetDocIdSet(IndexReader reader)
        {
            var bits = new OpenBitSet(reader.MaxDoc);
            TermDocs termDocs = reader.TermDocs();
            // Seek through the term docs to see if we find each term
            foreach (string assetId in _assetIds)
            {
                Term term = new Term(DocumentConstants.AssetId, assetId);
                termDocs.Seek(term);
                // There is only one document per issue so just get it.
                if (termDocs.Next())
                {
                    bits.Set(termDocs.Doc);
                }
            }

            return bits;
        }
    }
}