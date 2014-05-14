﻿namespace QuoteFlow.Models.CatalogImport
{
    public class CatalogRecordImportSkipped : ICatalogSummaryRecord
    {
        public CatalogRecordImportSkipped(int rowId)
        {
            RowId = rowId;
            Result = CatalogSummaryResult.Skip;
        }

        public int RowId { get; set; }
        public CatalogSummaryResult Result { get; set; }
    }
}