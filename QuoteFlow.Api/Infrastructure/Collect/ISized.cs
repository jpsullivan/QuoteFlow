namespace QuoteFlow.Api.Infrastructure.Collect
{
    /// <summary>
    /// Something that contains a number of items.
    /// </summary>
    public interface ISized
    {
        int Size();

        bool IsEmpty { get; }
    }
}