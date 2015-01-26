namespace QuoteFlow.Api.Models.CatalogImport
{
    public class CatalogRecordImportSkipped : ICatalogSummaryRecord
    {
        public CatalogRecordImportSkipped(int rowId)
        {
            RowId = rowId;
            Result = CatalogSummaryResult.Skip;
        }

        public CatalogRecordImportSkipped(int rowId, string reason)
        {
            RowId = rowId;
            Reason = reason;
            Result = CatalogSummaryResult.Skip;
        }

        public int Id { get; set; }
        public int RowId { get; set; }
        public CatalogSummaryResult Result { get; set; }

        /// <summary>
        /// The reason why this row failed to import.
        /// </summary>
        public string Reason { get; set; }
    }
}