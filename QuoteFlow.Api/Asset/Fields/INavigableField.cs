using System.Collections.Generic;
using Lucene.Net.Search;
using QuoteFlow.Api.Asset.Search;

namespace QuoteFlow.Api.Asset.Fields
{
    /// <summary>
    /// Fields in QuoteFlow which are able to be placed in the Asset Navigator implement this interface.
    /// </summary>
    public interface INavigableField : IField
    {
        string ColumnHeadingKey { get; }

        string ColumnCssClass { get; }

        /// <summary>
        /// The order in which to sort the field when it is sorted for the first time.
        /// </summary>
        /// <returns>  Either <see cref="#ORDER_ASCENDING"/> or <see cref="#ORDER_DESCENDING"/> </returns>
        string DefaultSortOrder { get; }

        /// <summary>
        /// A sortComparatorSource object to be used for sorting columns in a table.  In most cases this will use a
        /// <see cref="com.atlassian.jira.issue.search.parameters.lucene.sort.MappedSortComparator"/> using the <see cref="#getSorter()"/>
        /// method.  However, fields can provide any sorting mechanism that they wish.
        /// 
        /// If a field can be sorted directly using terms in the Lucene index then the field should implement <see cref="#getSortFields(boolean sortOrder)"/>
        /// rather than this method.
        /// 
        /// In large installations custom sorting may incur a maor performance penalty.
        /// </summary>
        /// <returns>  A SortComparatorSource that can be used to sort, or null if this field does not use custom sorting </returns>
        FieldComparatorSource SortComparatorSource { get; }

        /// <summary>
        /// Return a list of Lucene SortFields to be used for sorting search results.
        /// 
        /// Using this method allows the field to specify the most performant way to perform a search.  If a field can be
        /// sorted directly using the term in the index then this should just return a singleton list with the sort field.
        /// <para>
        /// {@code return Collections.singletonList(new SortField(fieldName, sortOrder));  }
        /// </para>
        /// 
        /// The default implementation builds this using the FieldComparatorSource returned by <see cref="#getSortComparatorSource()"/>
        /// 
        /// If you implement this method there is no need to implement <see cref="#getSortComparatorSource()"/>.
        /// </summary>
        /// <returns>The name of the indexed term to be used for native Lucene sorting.</returns>
        IEnumerable<SortField> GetSortFields(bool sortOrder);

        /// <summary>
        /// A sorter to be used when sorting columns in a table.  This sort uses the Lucene Document Collection
        /// and is therefore a lot faster than sorting the issues in memory.
        /// </summary>
        /// <returns>A sorter that can be used to sort this field, or null depending on the value of <see cref="SortComparatorSource"/></returns>
        ILuceneFieldSorter<object> Sorter { get; } 

        /// <summary>
        /// Returns the id of the field to check for visibility. For example, with original estimate field
        /// need to ensure that the timetracking field is not hidden. With most fields, this is the same as their
        /// id.
        /// </summary>
        string HiddenFieldId { get; }

//        string PrettyPrintChangeHistory(string changeHistory);
//
//        /// <summary>
//        /// Used for email notification templates - allows changelog to be displayed in language of the recipient. </summary>
//        /// <param name="changeHistory"> </param>
//        /// <returns> String   change history formatted according to locale in i18nHelper </returns>
//        string PrettyPrintChangeHistory(string changeHistory, I18nHelper i18nHelper);
    }

    public interface INavigableField<T> : INavigableField
    {
        /// <summary>
        /// A sorter to be used when sorting columns in a table.  This sort uses the Lucene Document Collection
        /// and is therefore a lot faster than sorting the issues in memory.
        /// </summary>
        /// <returns>A sorter that can be used to sort this field, or null depending on the value of <see cref="SortComparatorSource"/></returns>
        new ILuceneFieldSorter<T> Sorter { get; } 
    }
}