namespace QuoteFlow.Services.Interfaces
{
    public interface ICloudBlobClient
    {
        ICloudBlobContainer GetContainerReference(string containerAddress);
    }
}