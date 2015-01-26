using Lucene.Net.Documents;
using QuoteFlow.Api.Asset.Index.Indexers.Phrase;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Asset.Index.Indexers
{
    /// <summary>
    /// Responsible for populating a <seealso cref="Document"/> with the information held in
    /// the &quot;Name&quot; field of the <seealso cref="Asset"/>.
    /// </summary>
    public class AssetNameIndexer : BaseFieldIndexer
    {
		public virtual string Id
		{
			get { return SystemSearchConstants.ForSummary().FieldId; }
		}

		public virtual string DocumentFieldId
		{
			get { return SystemSearchConstants.ForSummary().IndexField; }
		}

		public override bool IsFieldVisibleAndInScope(Models.Asset asset)
		{
			return true;
		}

		public override void AddIndex(Document doc, Models.Asset asset)
		{
			IndexText(doc, DocumentFieldId, asset.Name, asset);
			IndexText(doc, PhraseQuerySupportField.ForIndexField(DocumentFieldId), asset.Name, asset);
			IndexTextForSorting(doc, DocumentConstants.AssetSortName, asset.Name, asset);
		}
    }
}