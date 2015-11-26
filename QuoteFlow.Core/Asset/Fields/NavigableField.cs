using System.Collections.Generic;
using Lucene.Net.Search;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Parameters.Lucene.Sort;

namespace QuoteFlow.Core.Asset.Fields
{
    public abstract class NavigableField : AbstractField, INavigableField
    {
        public NavigableField(string id, string name, string columnHeadingKey) 
            : base(id, name)
        {
            ColumnHeadingKey = columnHeadingKey;
            DefaultSortOrder = null;
        }

        public NavigableField(string id, string name, string columnHeadingKey, string defaultSortOrder)
            : base(id, name)
        {
            ColumnHeadingKey = columnHeadingKey;
            DefaultSortOrder = defaultSortOrder;
        }

        public string ColumnHeadingKey { get; }
        public string ColumnCssClass => Id;
        public string DefaultSortOrder { get; }

        public FieldComparatorSource SortComparatorSource => Sorter == null ? null : new MappedSortComparator(Sorter);

        public IEnumerable<SortField> GetSortFields(bool sortOrder)
        {
            throw new System.NotImplementedException();
        }
        
        public string HiddenFieldId { get; }
        public ILuceneFieldSorter<object> Sorter { get; }
    }
}