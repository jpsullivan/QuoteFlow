using Lucene.Net.Documents;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Asset.Index.Indexers.Phrase;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Asset.Index.Indexers
{
    /// <summary>
    /// Responsible for populating a <see cref="Document"/> with the information held in
    /// the &quot;Name&quot; field of the <see cref="Asset"/>.
    /// </summary>
    public class AssetNameIndexer : BaseFieldIndexer
    {
        public override string Id
		{
			get { return SystemSearchConstants.ForSummary().FieldId; }
		}

        public override string DocumentFieldId
		{
			get { return SystemSearchConstants.ForSummary().IndexField; }
		}

		public override bool IsFieldVisibleAndInScope(Api.Models.Asset asset)
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