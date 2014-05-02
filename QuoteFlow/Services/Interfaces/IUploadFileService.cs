using System.IO;
using System.Threading.Tasks;

namespace QuoteFlow.Services.Interfaces
{
    public interface IUploadFileService
    {
        Task DeleteUploadFileAsync(int userId);

        Task<Stream> GetUploadFileAsync(int userId);

        Task SaveUploadFileAsync(int userId, Stream fileStream, string fileExtension);
    }
}