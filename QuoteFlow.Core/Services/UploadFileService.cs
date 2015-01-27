using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using QuoteFlow.Api;
using QuoteFlow.Api.Services;
using QuoteFlow.Api.Upload;

namespace QuoteFlow.Services
{
    public class UploadFileService : IUploadFileService
    {
        private readonly IFileStorageService _fileStorageService;

        public UploadFileService(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        public Task DeleteUploadFileAsync(int userId)
        {
            if (userId < 1)
            {
                throw new ArgumentException("A user id is required.", "userId");
            }

            var uploadFileName = BuildFileName(userId);

            return _fileStorageService.DeleteFileAsync(Constants.UploadsFolderName, uploadFileName);
        }

        public Task<Stream> GetUploadFileAsync(int userId)
        {
            if (userId < 1)
            {
                throw new ArgumentException("A user id is required.", "userId");
            }

            // Use the trick of a private core method that actually does the async stuff to allow for sync arg contract checking
            return GetUploadFileAsyncCore(userId);
        }

        public Task SaveUploadFileAsync(int userId, Stream fileStream, string fileExtension, UploadType type, string filename = null)
        {
            if (userId < 1)
            {
                throw new ArgumentException("A user id is required.", "userId");
            }

            if (fileStream == null)
            {
                throw new ArgumentNullException("fileStream");
            }

            if (fileExtension == null) 
            {
                throw new ArgumentNullException("fileExtension");
            }

            if (filename == null)
            {
                filename = BuildFileName(userId, fileExtension);
            }

            return _fileStorageService.SaveFileAsync(GetUploadFolder(type), filename, fileStream);
        }

        /// <summary>
        /// Gets the upload folder name based on the specified upload type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string GetUploadFolder(UploadType type)
        {
            switch (type)
            {
                case UploadType.AssetImage:
                    return "asset-images";
                case UploadType.Catalog:
                    return "catalogs";
                case UploadType.ManufacturerLogo:
                    return "manufacturer-logos";
                default:
                    return Constants.UploadsFolderName;
            }
        }

        private static string BuildFileName(int userId, string extension = null)
        {
            if (extension == null) 
            {
                extension = ".csv";
            }

            return String.Format(CultureInfo.InvariantCulture, Constants.UploadFileNameTemplate, userId, extension);
        }

        // Use the trick of a private core method that actually does the async stuff to allow for sync arg contract checking
        private async Task<Stream> GetUploadFileAsyncCore(int userId)
        {
            var uploadFileName = BuildFileName(userId);
            return await _fileStorageService.GetFileAsync(Constants.UploadsFolderName, uploadFileName);
        }
    }
}