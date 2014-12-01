﻿using System;
using System.Collections.Generic;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Models.Search.Jql.Util
{
    public class JqlAssetSupport : IJqlAssetSupport
    {
        public IAssetService AssetService { get; protected set; }

        public JqlAssetSupport(IAssetService assetService)
        {
            if (assetService == null)
            {
                throw new ArgumentNullException("assetService");
            }

            AssetService = assetService;
        }

        public IAsset GetAsset(int id, User user)
        {
            // todo: add a permission based fetch method as well
            return AssetService.GetAsset(id);
        }

        public IAsset GetAsset(int id)
        {
            return AssetService.GetAsset(id);
        }

        public ISet<KeyValuePair<int, string>> GetCatalogManufacturerPairsByIds(ISet<int> assetIds)
        {
            throw new NotImplementedException();
        }

        public ISet<int> GetIdsOfMissingAssets(ISet<int> issueIds)
        {
            throw new NotImplementedException();
        }
    }
}