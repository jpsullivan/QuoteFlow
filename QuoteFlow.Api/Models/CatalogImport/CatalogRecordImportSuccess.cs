namespace QuoteFlow.Api.Models.CatalogImport
{
    public class CatalogRecordImportSuccess : ICatalogSummaryRecord
    {
        public CatalogRecordImportSuccess(int rowId, Asset asset)
        {
            RowId = rowId;
            AssetId = asset.Id;
            Result = CatalogSummaryResult.Success;
        }

        public CatalogRecordImportSuccess(int rowId, int assetId)
        {
            RowId = rowId;
            AssetId = assetId;
            Result = CatalogSummaryResult.Success;
        }

        public int Id { get; set; }
        public int RowId { get; set; }
        
        public CatalogSummaryResult Result { get; set; }

        public int AssetId { get; set; }
    }
}