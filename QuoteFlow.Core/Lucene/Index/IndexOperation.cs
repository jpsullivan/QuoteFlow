using QuoteFlow.Api.Lucene.Index;

namespace QuoteFlow.Core.Lucene.Index
{
    /// <summary>
    /// An operation that is performed on an Index. See <seealso cref="Operations"/> for
    /// factory methods.
    /// 
    /// Note: this is not an interface to prevent clients implementing it. All
    /// clients need to now is that they have an <seealso cref="IndexOperation"/> that will do a
    /// create/delete/update/whatever, not how it is implemented.
    /// </summary>
    public abstract class IndexOperation
    {
        internal IndexOperation()
        {
        }

        public abstract void Perform(IWriter writer);

        public abstract UpdateMode Mode { get; set; }
    }
}
