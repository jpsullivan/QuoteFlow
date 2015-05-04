using System;
using System.Collections;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Asset.Fields.Layout.Column
{
    public interface IColumnLayoutItem : IComparable
    {
        /// <summary>
        /// Transform to its id.
        /// </summary>
        INavigableField NavigableField { get; }

        /// <summary>
        /// Return the string form of the unique identifier for this column. When the column corresponds to a {@link
        /// NavigableField}, the id of the column will be the same as the id of the field.
        /// </summary>
        /// <returns> the id; </returns>
        string Id { get; }

        bool IsAliasForField(User user, string sortField);

        int Position { get; }

        string GetHtml(IDictionary displayParams, IAsset asset);

        /// <summary>
        /// Return some text for the Column Header. By default this calls <see cref="NavigableField#getColumnHeadingKey"/> but
        /// implementations can override this to provide different column headings as appropriate
        /// </summary>
        string ColumnHeadingKey { get; }
    }
}