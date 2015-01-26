using System.Collections.Generic;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Services
{
    public interface IAssetVarService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AssetVar GetAssetVar(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AssetVarValue GetVarValue(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        IEnumerable<AssetVar> GetAssetVarsByOrganizationId(int organizationId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        IEnumerable<AssetVarValue> GetVarValues(int assetId);

        /// <summary>
        /// Fetches a collection of <see cref="AssetVar"/> objects which also contains
        /// their respective <see cref="AssetVarValue"/>.
        /// </summary>
        /// <param name="assetId">The asset whose asset vars will be searched for.</param>
        /// <returns></returns>
        IEnumerable<AssetVar> GetAssetVarsWithValues(int assetId); 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetVar"></param>
        void InsertAssetVar(AssetVar assetVar);

        /// <summary>
        /// Updates an asset var value record.
        /// </summary>
        /// <param name="assetVar"></param>
        void UpdateAssetVar(AssetVar assetVar);

        /// <summary>
        /// Updates an asset var value record, but only specific fields.
        /// </summary>
        /// <param name="assetVarValueId"></param>
        /// <param name="assetVarId"></param>
        /// <param name="assetVarValue"></param>
        void UpdateAssetVarValue(int assetVarValueId, int assetVarId, string assetVarValue);

        /// <summary>
        /// Deletes an asset var record based on its ID.
        /// </summary>
        /// <param name="id"></param>
        void DeleteAssetVar(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="varValue"></param>
        int? InsertVarValue(AssetVarValue varValue);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="varValue"></param>
        void UpdateAssetVarValue(AssetVarValue varValue);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        void DeleteAssetVarValue(int id);
    }
}