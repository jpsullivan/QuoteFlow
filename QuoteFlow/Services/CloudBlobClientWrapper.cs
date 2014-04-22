using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Services
{
    public class CloudBlobClientWrapper : ICloudBlobClient
    {
        private readonly string _storageConnectionString;
        private CloudBlobClient _blobClient;

        public CloudBlobClientWrapper(string storageConnectionString)
        {
            _storageConnectionString = storageConnectionString;
        }

        public ICloudBlobContainer GetContainerReference(string containerAddress)
        {
            if (_blobClient == null)
            {
                _blobClient = CloudStorageAccount.Parse(_storageConnectionString).CreateCloudBlobClient();
            }
            return new CloudBlobContainerWrapper(_blobClient.GetContainerReference(containerAddress));
        }
    }
}