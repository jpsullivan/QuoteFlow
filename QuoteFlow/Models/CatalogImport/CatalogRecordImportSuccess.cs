namespace QuoteFlow.Models.CatalogImport
{
    public class CatalogRecordImportSuccess : ICatalogSummaryRecord
    {
        public CatalogRecordImportSuccess(int rowId, Asset asset)
        {
            RowId = rowId;
            Asset = asset;
            Result = CatalogSummaryResult.Success;
        }

        public int RowId { get; set; }
        
        public CatalogSummaryResult Result { get; set; }

        public Asset Asset { get; set; }
    }
}