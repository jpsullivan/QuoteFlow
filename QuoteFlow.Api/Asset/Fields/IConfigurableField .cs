using System.Collections.Generic;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Asset.Fields
{
    /// <summary>
    /// ConfigurableField are fields which have <see cref="FieldConfigItemType"/> that can be stored.
    /// </summary>
    public interface IConfigurableField : IOrderableField
    {
        /// <summary>
        /// Returns a list of projects associated with this field. Will be null if the field is global
        /// </summary>
        /// <returns> a list of projects associated with this field. </returns>
        IEnumerable<Catalog> AssociatedCatalogs { get; }
    }
}