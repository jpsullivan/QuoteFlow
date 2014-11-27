using System.Collections.Generic;
using QuoteFlow.Models.Assets.Index;
using QuoteFlow.Models.Assets.Search;
using QuoteFlow.Models.Assets.Search.Handlers;
using QuoteFlow.Models.Assets.Statistics;

namespace QuoteFlow.Models.Assets.Fields
{
    public class SummarySystemField : AbstractTextSystemField, INavigableField, ISummaryField
    {
        private const string SUMMARY_NAME_KEY = "issue.field.summary";
        private static readonly ILuceneFieldSorter<string> SORTER = new TextFieldSorter(DocumentConstants.AssetSortName);

        public SummarySystemField(SummarySearchHandlerFactory searchHandlerFactory)
            : base(AssetFieldConstants.Summary, SUMMARY_NAME_KEY, searchHandlerFactory)
        {
        }

        void IOrderableField.PopulateFromAsset(IDictionary<string, object> fieldValuesHolder, Asset asset)
        {
            fieldValuesHolder[base.Id] = asset.Name;
        }
    }
}