namespace QuoteFlow.Models.CatalogImport
{
    public class CatalogRecordImportFailure : ICatalogSummaryRecord
    {
        public CatalogRecordImportFailure(int rowId, string reason)
        {
            RowId = rowId;
            Reason = reason;
            Result = CatalogSummaryResult.Failure;
        }

        public CatalogRecordImportFailure(int rowId, string reason, int assetId)
        {
            RowId = rowId;
            AssetId = assetId;
            Reason = reason;
            Result = CatalogSummaryResult.Failure;
        }

        public int Id { get; set; }
        public int RowId { get; set; }
        public CatalogSummaryResult Result { get; set; }

        /// <summary>
        /// The asset id this failure is relative to.
        /// </summary>
        public int AssetId { get; set; }

        /// <summary>
        /// The reason why this row failed to import.
        /// </summary>
        public string Reason { get; set; }
    }
}