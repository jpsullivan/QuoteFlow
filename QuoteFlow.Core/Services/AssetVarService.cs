﻿using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Core.Services
{
    public class AssetVarService : IAssetVarService
    {
        public AssetVar GetAssetVar(int id)
        {
            if (id == 0)
                throw new ArgumentException("Asset Var ID must be greater than zero", nameof(id));

            return Current.DB.AssetVars.Get(id);
        }

        public AssetVarValue GetVarValue(int id)
        {
            if (id == 0)
                throw new ArgumentException("Asset Var Value ID must be greater than zero", nameof(id));

            return Current.DB.AssetVarValues.Get(id);
        }

        public IEnumerable<AssetVar> GetAssetVarsByOrganizationId(int organizationId)
        {
            const string sql = "select * from AssetVars where OrganizationId = @organizationId";
            return Current.DB.Query<AssetVar>(sql, new {organizationId});
        }

        public IEnumerable<AssetVarValue> GetVarValues(int assetId)
        {
            if (assetId == 0)
                throw new ArgumentException("Asset ID must be greater than zero", nameof(assetId));

            const string sql =
                "select * from AssetVarValues where AssetId = @assetId";
            return Current.DB.Query<AssetVarValue>(sql, new {assetId});
        }

        /// <summary>
        /// Fetches a collection of <see cref="AssetVar"/> objects which also contains
        /// their respective <see cref="AssetVarValue"/>.
        /// </summary>
        /// <param name="assetId">The asset whose asset vars will be searched for.</param>
        /// <returns></returns>
        public IEnumerable<AssetVar> GetAssetVarsWithValues(int assetId)
        {
            var values = GetVarValues(assetId).ToList();
            var varIds = values.Select(v => v.AssetVarId).ToList();

            const string sql = "select * from AssetVars where Id in @varIds";

            var assetVars = Current.DB.Query<AssetVar>(sql, new {varIds}).ToList();

            foreach (var assetVar in assetVars) {
                assetVar.Value = values.Single(value => value.AssetVarId == assetVar.Id);
            }

            return assetVars;
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
                throw new ArgumentException("AssetVar ID must be greater than zero.", nameof(id));
            }

            Current.DB.AssetVars.Delete(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="varValue"></param>
        /// <returns></returns>
        public int? InsertVarValue(AssetVarValue varValue)
        {
            if (varValue == null)
            {
                throw new ArgumentException("Asset var value cannot be null.");
            }

            return Current.DB.AssetVarValues.Insert(varValue);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="varValue"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetVarValueId"></param>
        /// <param name="assetVarId"></param>
        /// <param name="assetVarValue"></param>
        public void UpdateAssetVarValue(int assetVarValueId, int assetVarId, string assetVarValue)
        {
            if (assetVarValueId == 0) throw new ArgumentException("Asset Var Value ID must be greater than zero.", nameof(assetVarValueId));
            if (assetVarId == 0) throw new ArgumentException("Asset Var row ID must be greater than zero.", nameof(assetVarId));

            Current.DB.AssetVarValues.Update(assetVarValueId, new
            {
                VarValue = assetVarValue,
                AssetVarId = assetVarId
            });
        }

        public void DeleteAssetVarValue(int id)
        {
            if (id == 0)
            {
                throw new ArgumentException("AssetVarValue ID must be greater than zero.", nameof(id));
            }

            Current.DB.AssetVarValues.Delete(id);
        }
    }
}