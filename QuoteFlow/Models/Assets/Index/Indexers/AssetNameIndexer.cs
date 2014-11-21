using Lucene.Net.Documents;
using QuoteFlow.Models.Assets.Index.Indexers.Phrase;
using QuoteFlow.Models.Assets.Search.Constants;

namespace QuoteFlow.Models.Assets.Index.Indexers
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

		public override bool IsFieldVisibleAndInScope(Asset asset)
		{
			return true;
		}

		public override void AddIndex(Document doc, Asset asset)
		{
			IndexText(doc, DocumentFieldId, asset.Name, asset);
			IndexText(doc, PhraseQuerySupportField.ForIndexField(DocumentFieldId), asset.Name, asset);
			IndexTextForSorting(doc, DocumentConstants.AssetSortName, asset.Name, asset);
		}
    }
}