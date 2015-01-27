using System.IO;
using System.Threading.Tasks;
using QuoteFlow.Api.Upload;

namespace QuoteFlow.Api.Services
{
    public interface IUploadFileService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task DeleteUploadFileAsync(int userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Stream> GetUploadFileAsync(int userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fileStream"></param>
        /// <param name="fileExtension"></param>
        /// <param name="type"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        Task SaveUploadFileAsync(int userId, Stream fileStream, string fileExtension, UploadType type, string filename = "");
    }
}