using QuoteFlow.Api.Util;
using Wintellect.PowerCollections;

namespace QuoteFlow.Core.Util
{
    /// <summary>
    /// Is a message set that keeps the messages and warnings in the order in which they were added.
    /// </summary>
    public class ListOrderedMessageSet : AbstractMessageSet
    {
        public ListOrderedMessageSet() : base(new Set<string>(), new Set<string>())
        {
        }
    }
}