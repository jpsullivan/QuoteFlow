using System.Collections.Generic;

namespace QuoteFlow.Infrastructure
{
    /// <summary>
    /// Defines a domain specific data type. An example of this is Asset, Manufacturer, or Date.
    /// 
    /// Fields handle data of a specified type and searchers and functions know which
    /// data-types they can handle working on.
    /// </summary>
    public interface IQuoteFlowDataType
    {
        /// <summary>
        /// Provides a string representation of this QuoteFlowDataType's actual types. A QuoteFlowDataType 
        /// can declare that it is made up of distinct types and this is why we return a collection 
        /// of string representations.
        /// </summary>
        /// <returns>String representation of this QuoteFlowDataType's actual types.</returns>
        ICollection<string> AsStrings();

        /// <summary>
        /// Determines if this type matches the passed in other QuoteFlowDataType.
        /// 
        /// This method runs through the data types and will return true if any of the types are 
        /// equals to the other types.
        /// 
        /// This method should be reflexive, if a.match(b) == true then b.match(a) == true
        /// 
        /// There is a special case which is <see cref="object"/>. This means all and any comparison 
        /// against Object.class will return true for the match method.
        /// </summary>
        /// <param name="otherType">The data type to compare to, not null.</param>
        /// <returns>True if any of this types are assignable to the other types.</returns>
        bool Matches(IQuoteFlowDataType otherType);
    }
}