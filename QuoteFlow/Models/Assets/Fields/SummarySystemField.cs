using System.Collections.Generic;
using Lucene.Net.Search;
using QuoteFlow.Models.Assets.Index;
using QuoteFlow.Models.Assets.Search;
using QuoteFlow.Models.Assets.Search.Handlers;
using QuoteFlow.Models.Assets.Search.Parameters.Lucene.Sort;
using QuoteFlow.Models.Assets.Statistics;

namespace QuoteFlow.Models.Assets.Fields
{
    public class SummarySystemField : AbstractTextSystemField, INavigableField<string>, ISummaryField
    {
        private const string SUMMARY_NAME_KEY = "asset.field.summary";
        private static readonly ILuceneFieldSorter<string> SORTER = new TextFieldSorter(DocumentConstants.AssetSortName);

        public SummarySystemField(SummarySearchHandlerFactory searchHandlerFactory)
            : base(AssetFieldConstants.Summary, SUMMARY_NAME_KEY, searchHandlerFactory)
        {
        }

        void IOrderableField.PopulateFromAsset(IDictionary<string, object> fieldValuesHolder, Asset asset)
        {
            fieldValuesHolder[base.Id] = asset.Name;
        }

        ILuceneFieldSorter<string> INavigableField<string>.Sorter
        {
            get { return SORTER; }
        }

        IEnumerable<SortField> INavigableField.GetSortFields(bool sortOrder)
        {
            return new List<SortField>
            {
                new SortField(DocumentConstants.AssetSortName, new StringSortComparator(), sortOrder)
            };
        }
    }
}