namespace QuoteFlow.Api.Asset.Transport
{
    /// <summary>
    /// This contains String > Collection of Transport Objects
    /// </summary>
    public interface IFieldTransportParams : ICollectionParams
    {
        object GetFirstValueForNullKey();
        object GetFirstValueForKey(string key);
    }
}