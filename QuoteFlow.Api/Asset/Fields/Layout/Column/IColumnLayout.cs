using System.Collections.Generic;

namespace QuoteFlow.Api.Asset.Fields.Layout.Column
{
    /// <summary>
    /// Represents the cause or source of columns in an asset table, e.g. whether they were requested explicitly, configured
    /// as the columns of a filter or the user's configured defaults.
    /// </summary>
    public interface IColumnLayout
    {
        IEnumerable<IColumnLayoutItem> ColumnLayoutItems { get; }

        bool Contains(INavigableField navigableField);

        /// <summary>
        /// Returns the column layout items as a list of string.
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> AsFieldNames();

        /// <summary>
        /// Returns the columns used when creating the ColumnLayout.
        /// </summary>
        ColumnConfig ColumnConfig { get; }
    }
}