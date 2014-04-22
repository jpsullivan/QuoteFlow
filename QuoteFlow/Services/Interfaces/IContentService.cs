using System;
using System.Threading.Tasks;
using System.Web;

namespace QuoteFlow.Services.Interfaces
{
    public interface IContentService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="expiresIn"></param>
        /// <returns></returns>
        Task<IHtmlString> GetContentItemAsync(string name, TimeSpan expiresIn);
    }
}