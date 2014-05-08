using System.Collections.Generic;
using QuoteFlow.Models;

namespace QuoteFlow.Services.Interfaces
{
    public interface IAssetVarService
    {
        AssetVar GetAssetVar(int id);

        AssetVarValue GetVarValue(int id);

        IEnumerable<AssetVar> GetAssetVarsByOrganizationId(int organizationId);

        IEnumerable<AssetVarValue> GetVarValues(int assetId, int organizationId);

        void InsertAssetVar(AssetVar assetVar);

        void InsertVarValue(AssetVarValue varValue);
    }
}