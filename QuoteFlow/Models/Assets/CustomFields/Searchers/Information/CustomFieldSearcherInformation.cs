using System.Collections.Generic;
using QuoteFlow.Models.Assets.Fields;
using QuoteFlow.Models.Assets.Index.Indexers;
using QuoteFlow.Models.Assets.Search.Searchers;
using QuoteFlow.Models.Assets.Search.Searchers.Information;

namespace QuoteFlow.Models.Assets.CustomFields.Searchers.Information
{
    public class CustomFieldSearcherInformation : GenericSearcherInformation<ICustomField>
    {
        public CustomFieldSearcherInformation(string id, string nameKey, ICustomField field, IEnumerable<IFieldIndexer> relatedIndexers, SearcherGroupType searcherGroupType) : base(id, nameKey, field, relatedIndexers, searcherGroupType)
        {
        }
    }
}