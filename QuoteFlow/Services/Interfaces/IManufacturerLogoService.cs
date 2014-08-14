using System;
using QuoteFlow.Models;

namespace QuoteFlow.Services.Interfaces
{
    public interface IManufacturerLogoService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="manufacturerId"></param>
        /// <returns></returns>
        ManufacturerLogo Get(int manufacturerId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manufacturerId"></param>
        /// <param name="token"></param>
        void Insert(int manufacturerId, Guid token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manufacturerId"></param>
        void Delete(int manufacturerId);
    }
}