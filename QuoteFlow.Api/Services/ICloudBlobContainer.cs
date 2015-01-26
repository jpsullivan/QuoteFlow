using System.Threading.Tasks;

namespace QuoteFlow.Api.Services
{
    public interface ICloudBlobContainer
    {
        Task CreateIfNotExistAsync();
        Task SetPermissionsAsync(BlobContainerPermissions permissions);
        ISimpleCloudBlob GetBlobReference(string blobAddressUri);
    }
}
