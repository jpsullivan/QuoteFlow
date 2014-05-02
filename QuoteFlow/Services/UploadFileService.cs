using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using QuoteFlow.Services.Interfaces;

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

        public Task SaveUploadFileAsync(int userId, Stream fileStream, string fileExtension)
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

            var uploadFileName = BuildFileName(userId, fileExtension);
            return _fileStorageService.SaveFileAsync(Constants.UploadsFolderName, uploadFileName, fileStream);
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