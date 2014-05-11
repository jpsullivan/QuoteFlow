using System;
using System.Collections.Generic;
using QuoteFlow.Models;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Services
{
    public class AssetVarService : IAssetVarService
    {
        public AssetVar GetAssetVar(int id)
        {
            if (id == 0)
                throw new ArgumentException("Asset Var ID must be greater than zero", "id");

            return Current.DB.AssetVars.Get(id);
        }

        public AssetVarValue GetVarValue(int id)
        {
            if (id == 0)
                throw new ArgumentException("Asset Var Value ID must be greater than zero", "id");

            return Current.DB.AssetVarValues.Get(id);
        }

        public IEnumerable<AssetVar> GetAssetVarsByOrganizationId(int organizationId)
        {
            const string sql = "select * from AssetVars where OrganizationId = @organizationId";
            return Current.DB.Query<AssetVar>(sql, new {organizationId});
        }

        public IEnumerable<AssetVarValue> GetVarValues(int assetId, int organizationId)
        {
            if (assetId == 0)
                throw new ArgumentException("Asset ID must be greater than zero", "assetId");

            if (organizationId == 0)
                throw new ArgumentException("Organization ID must be greater than zero", "organizationId");

            const string sql =
                "select * from AssetVarValues where AssetId = @assetId and OrganizationId = @organizationId";
            return Current.DB.Query<AssetVarValue>(sql, new {assetId, organizationId});
        }

        public void InsertAssetVar(AssetVar assetVar)
        {
            if (assetVar == null)
            {
                throw new ArgumentException("Asset var cannot be null.");
            }

            Current.DB.AssetVars.Insert(assetVar);
        }

        public void UpdateAssetVar(AssetVar assetVar)
        {
            if (assetVar == null) 
            {
                throw new ArgumentException("Asset var cannot be null.");
            }

            if (assetVar.Id == 0) 
            {
                throw new ArgumentException("Asset var must have an ID greater than zero.");
            }

            Current.DB.AssetVars.Update(assetVar.Id, assetVar);
        }

        public void DeleteAssetVar(int id)
        {
            if (id == 0) 
            {
                throw new ArgumentException("AssetVar ID must be greater than zero.", "id");
            }

            Current.DB.AssetVars.Delete(id);
        }

        public void InsertVarValue(AssetVarValue varValue)
        {
            if (varValue == null)
            {
                throw new ArgumentException("Asset var value cannot be null.");
            }

            Current.DB.AssetVarValues.Insert(varValue);
        }

        public void UpdateAssetVarValue(AssetVarValue varValue)
        {
            if (varValue == null)
            {
                throw new ArgumentException("Asset var value cannot be null.");
            }

            if (varValue.Id == 0)
            {
                throw new ArgumentException("Asset var value must have an ID greater than zero.");
            }

            Current.DB.AssetVarValues.Update(varValue.Id, varValue);
        }

        public void DeleteAssetVarValue(int id)
        {
            if (id == 0)
            {
                throw new ArgumentException("AssetVarValue ID must be greater than zero.", "id");
            }

            Current.DB.AssetVarValues.Delete(id);
        }
    }
}