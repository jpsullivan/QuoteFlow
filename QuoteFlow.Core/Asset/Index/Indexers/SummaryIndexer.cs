﻿using Lucene.Net.Documents;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Asset.Index.Indexers.Phrase;
using QuoteFlow.Api.Asset.Search.Constants;

namespace QuoteFlow.Core.Asset.Index.Indexers
{
    /// <summary>
    /// Responsible for populating a <see cref="Document"/> lucene document with the information held in
    /// the &quot;Summary&quot; field of the <see cref="Asset asset"/>
    /// </summary>
    public class SummaryIndexer : BaseFieldIndexer
    {
        public override string Id => SystemSearchConstants.ForSummary().FieldId;

        public override string DocumentFieldId => SystemSearchConstants.ForSummary().IndexField;

        public override bool IsFieldVisibleAndInScope(Api.Models.Asset issue)
        {
            return true;
        }

        public override void AddIndex(Document doc, Api.Models.Asset asset)
        {
            IndexText(doc, DocumentFieldId, asset.Name, asset);
            IndexText(doc, PhraseQuerySupportField.ForIndexField(DocumentFieldId), asset.Name, asset);
            IndexTextForSorting(doc, DocumentConstants.AssetSortName, asset.Name, asset);
        }
    }
}