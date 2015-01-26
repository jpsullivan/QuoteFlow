using System;
using System.Threading.Tasks;
using System.Web;

namespace QuoteFlow.Api.Services
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