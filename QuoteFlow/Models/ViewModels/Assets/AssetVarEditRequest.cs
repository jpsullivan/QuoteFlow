namespace QuoteFlow.Models.ViewModels.Assets
{
    public class AssetVarEditRequest
    {
        public AssetVarEditRequest(int assetVarValueId, int assetVarId, string assetVarValue)
        {
            AssetVarValueId = assetVarValueId;
            AssetVarId = assetVarId;
            AssetVarValue = assetVarValue;
        }

        /// <summary>
        /// The row ID of the asset var value.
        /// </summary>
        public int AssetVarValueId { get; set; }

        /// <summary>
        /// The row ID of the asset var itself.
        /// </summary>
        public int AssetVarId { get; set; }

        /// <summary>
        /// The value of the asset var value (for lack of a better descriptor).
        /// </summary>
        public string AssetVarValue { get; set; }
    }
}