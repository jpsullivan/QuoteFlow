using Wintellect.PowerCollections;

namespace QuoteFlow.Infrastructure.Util
{
    public class MessageSet : AbstractMessageSet
    {
        public MessageSet() : base(new Set<string>(), new Set<string>())
        {
        }
    }
}