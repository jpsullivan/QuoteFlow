using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuoteFlow.Models;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Services
{
    public class ManufacturerLogoService : IManufacturerLogoService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="manufacturerId"></param>
        /// <returns></returns>
        public ManufacturerLogo Get(int manufacturerId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manufacturerId"></param>
        /// <param name="token"></param>
        public void Insert(int manufacturerId, Guid token)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manufacturerId"></param>
        public void Delete(int manufacturerId)
        {
            throw new NotImplementedException();
        }
    }
}