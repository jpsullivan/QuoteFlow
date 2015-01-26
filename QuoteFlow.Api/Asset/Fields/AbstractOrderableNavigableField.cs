using System.Collections.Generic;
using Lucene.Net.Search;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Handlers;
using QuoteFlow.Api.Asset.Search.Parameters.Lucene.Sort;

namespace QuoteFlow.Api.Asset.Fields
{
    public abstract class AbstractOrderableNavigableField : AbstractOrderableField, INavigableField
    {
        public string ColumnHeadingKey { get; private set; }
        public string ColumnCssClass { get; private set; }
        public string DefaultSortOrder { get; private set; }
        public ILuceneFieldSorter<object> Sorter { get; private set; }
        public FieldComparatorSource SortComparatorSource { get { return new MappedSortComparator(Sorter); } }
        public string HiddenFieldId { get; private set; }

        public AbstractOrderableNavigableField(string id, string name, ISearchHandlerFactory searcherHandlerFactory)
            : base(id, name, searcherHandlerFactory)
        {
        }

        public IEnumerable<SortField> GetSortFields(bool sortOrder)
        {
            // if Sorter returns null here, then we are already in a whole heap of pain.
            var fieldName = Sorter.DocumentConstant;
            return new List<SortField> { new SortField(fieldName, SortComparatorSource, sortOrder) };
        }
    }
}