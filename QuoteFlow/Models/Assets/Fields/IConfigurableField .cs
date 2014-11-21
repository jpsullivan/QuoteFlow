using System.Collections.Generic;

namespace QuoteFlow.Models.Assets.Fields
{
    /// <summary>
    /// ConfigurableField are fields which have <seealso cref="FieldConfigItemType"/> that can be stored.
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