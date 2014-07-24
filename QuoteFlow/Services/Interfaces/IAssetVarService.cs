using System.Collections.Generic;
using QuoteFlow.Models;

namespace QuoteFlow.Services.Interfaces
{
    public interface IAssetVarService
    {
        AssetVar GetAssetVar(int id);

        AssetVarValue GetVarValue(int id);

        IEnumerable<AssetVar> GetAssetVarsByOrganizationId(int organizationId);

        IEnumerable<AssetVarValue> GetVarValues(int assetId);

        /// <summary>
        /// Fetches a collection of <see cref="AssetVar"/> objects which also contains
        /// their respective <see cref="AssetVarValue"/>.
        /// </summary>
        /// <param name="assetId">The asset whose asset vars will be searched for.</param>
        /// <returns></returns>
        IEnumerable<AssetVar> GetAssetVarsWithValues(int assetId); 

        void InsertAssetVar(AssetVar assetVar);

        void UpdateAssetVar(AssetVar assetVar);

        void DeleteAssetVar(int id);

        void InsertVarValue(AssetVarValue varValue);

        void UpdateAssetVarValue(AssetVarValue varValue);

        void DeleteAssetVarValue(int id);
    }
}