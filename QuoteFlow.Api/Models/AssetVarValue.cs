namespace QuoteFlow.Api.Models
{
    public class AssetVarValue
    {
        public AssetVarValue() { }

        public AssetVarValue(int assetId, int assetVarId, string value, int organizationId)
        {
            AssetId = assetId;
            AssetVarId = assetVarId;
            VarValue = value;
            OrganizationId = organizationId;
        }

        public int Id { get; set; }

        public int AssetId { get; set; }

        public string VarValue { get; set; }

        public int AssetVarId { get; set; }

        public int OrganizationId { get; set; }
    }
}