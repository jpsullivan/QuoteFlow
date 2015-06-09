using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Jql.Context
{
    /// <summary>
    /// Represents a <see cref="Manufacturer"/> that is part of a search context.
    /// </summary>
    public interface IManufacturerContext
    {
        /// <summary>
        /// Returns the manufacturer id for this context element.
        /// </summary>
        int? ManufacturerId { get; }

        /// <summary>
        /// Indicates the special case of all manufacturers that are not enumerated. If this is true 
        /// then the value for <see cref="ManufacturerId"/> will be null.
        /// </summary>
        /// <returns>True if all, false otherwise.</returns>
        bool IsAll();
    }
}