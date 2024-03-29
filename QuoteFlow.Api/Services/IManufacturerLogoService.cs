﻿using System;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Services
{
    public interface IManufacturerLogoService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="manufacturerId"></param>
        /// <returns></returns>
        ManufacturerLogo GetByManufacturerId(int manufacturerId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manufacturerId"></param>
        /// <param name="token"></param>
        /// <param name="url"></param>
        void CreateManufacturerLogo(int manufacturerId, Guid token, string url);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manufacturerId"></param>
        void Delete(int manufacturerId);
    }
}