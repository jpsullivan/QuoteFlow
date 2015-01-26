namespace QuoteFlow.Api.Services
{
    public interface ICloudBlobClient
    {
        ICloudBlobContainer GetContainerReference(string containerAddress);
    }
}