namespace QuoteFlow.Models
{
    public class AssetVarValue
    {
        public int Id { get; set; }

        public int AssetId { get; set; }

        public string VarValue { get; set; }

        public int OrganizationId { get; set; }
    }
}