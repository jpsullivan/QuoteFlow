using System.Collections.Generic;

namespace QuoteFlow.Models.Assets.Transport
{
    /// <summary>
    /// Key is the field id, the value contain Strings of a undetermined dimension.
    /// </summary>
    public interface IFieldValuesHolder : IDictionary<string, object>
    {
    }
}