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

        public int RowId { get; set; }
        public CatalogSummaryResult Result { get; set; }

        /// <summary>
        /// The reason why this row failed to import.
        /// </summary>
        private string Reason { get; set; }
    }
}