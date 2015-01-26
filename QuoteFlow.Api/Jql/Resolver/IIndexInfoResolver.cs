using System.Collections.Generic;

namespace QuoteFlow.Api.Jql.Resolver
{
    /// <summary>
    /// Turns a searchable value (operand) (such as what may be typed into the right side of a clause) into an indexed value.
    /// Operands may be represented by name or some other format that doesn't exactly match the indexed value and
    /// implementations of this perform the transformation. In some cases (notably numeric types) there would be little or
    /// no transformation of the original value held by the operand.
    /// Multiple values are returned because lookups cannot be guaranteed to be 1:1 as in name to id lookups for
    /// certain issue fields.
    /// </summary>
    public interface IIndexInfoResolver<T>
    {
        /// <summary>
        /// Provides the values in the index for the operand with the given String value.
        /// </summary>
        /// <param name="rawValue">The value whose indexed term equivalent is to be returned.</param>
        /// <returns>The values to put or search for in the index, possibly empty, never containing null.</returns>
        List<string> GetIndexedValues(string rawValue);

        /// <summary>
        /// Provides the values in the index for the single value operand with the given Long value.
        /// </summary>
        /// <param name="rawValue">The value whose indexed term equivalent is to be returned.</param>
        /// <returns>The values to put or search for in the index, possibly empty, never containing null.</returns>
        List<string> GetIndexedValues(int? rawValue);

        /// <summary>
        /// Gets an indexed value from a domain object.
        /// </summary>
        /// <param name="indexedObject">The domain object. Does not accept null.</param>
        /// <returns>The indexed value.</returns>
        string GetIndexedValue(T indexedObject);
    }
}