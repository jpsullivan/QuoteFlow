using System.Collections.Generic;
using System.Linq;
using Dapper;
using Jil;

namespace QuoteFlow.Api.Auditing.DetailResolvers.Asset
{
    public class AssetUpdated : IDetailResolver
    {
        public AssetUpdated()
        {
        }

        public AssetUpdated(Models.Asset oldAsset, DynamicParameters changedParameters)
        {
            var changedFields = new Dictionary<string, string[]>();

            // strip out the LastUpdated property since we don't care if it was changed
            foreach (var p in changedParameters.ParameterNames)
            {
                if (p == "LastUpdated") continue;

                var oldValue = oldAsset.GetType().GetProperty(p).GetValue(oldAsset, null).ToString();
                var newValue = changedParameters.Get<object>(p).ToString();

                changedFields.Add(p, new[] {oldValue, newValue});
            }

            ChangedParameters = changedFields;
        }

        /// <summary>
        /// The parameters that were modified for a particular asset.
        /// </summary>
        public IDictionary<string, string[]> ChangedParameters { get; set; }

        public string Serialize()
        {
            return JSON.Serialize(this);
        }
    }
}
