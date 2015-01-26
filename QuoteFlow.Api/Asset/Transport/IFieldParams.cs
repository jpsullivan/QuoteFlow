using System.Collections.Generic;

namespace QuoteFlow.Api.Asset.Transport
{
    /// <summary>
    /// A parent interface for transport objects in QuoteFlow. All FieldParams share the logic of a 
    /// String key with a multi-dimensional value. e.g. array, lists, FieldParams. Keys may be null. 
    /// The interface does not mandate what objects the multi-dimensional value contain. This is up 
    /// to the implementers and sub-interfaces to mandate.
    /// </summary>
    public interface IFieldParams
    {
        /// <summary>
        /// 
        /// </summary>
        ICollection<string> AllKeys { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool ContainsKey(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool IsEmpty();
    }
}